using API.BaseRepository;
using Application.Helper.PlaceOrder;
using Application.Repository.IService;
using Application.Repository.Service.ShoppingProcessor.Interface;
using Application.ViewModel.Movie;
using Application.ViewModel.ShoppingCart;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShoppingCartItemController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IShoppingCart _shoppingCart;
        private readonly IShoppingDiscountHall _shoppingHall;
        /*private readonly IBaseRepository<ShoppingCartItem> _base;*/
         
        public ShoppingCartItemController(ApplicationDbContext context,
              UserManager<ApplicationUser> userManager, IShoppingCart shoppingCart, IShoppingDiscountHall shoppingHall
              /*IBaseRepository<ShoppingCartItem> @base*/)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCart = shoppingCart;
            _shoppingHall = shoppingHall;
            /*_base = @base;*/
        }
        [HttpGet]
        public async Task<ActionResult<MyShoppingCartItemsView>> MyShoppingCartItems()
        {
            var user = await _userManager.GetUserAsync(User);

            var cartList = await (from s in _context.Movies
                             join cart in _context.ShoppingCartItems on s.Id equals cart.MovieId
                             where cart.MyCartUserId ==user.Id
                             select new MyShoppingCartItemsInnerView
                             {
                                 SelectedAmount = cart.Amount,
                                 MovieId=cart.MovieId,
                                 MovieName=cart.Movie.Name,
                                 Price = Math.Round(cart.Price, 2),
                                 IsSelected = cart.IsSelected,
                                 SubTotal = Math.Round(cart.Amount * cart.Price, 2),
                                 DiscountedPrice = s.Price > cart.Price ? Math.Round(s.Price - cart.Price, 2) : 0.00 
                             }).ToListAsync();

            await _shoppingCart.RefreshCart(user.Id);

            /*var now = DateTime.Now;

            var allCart = await _context.ShoppingCartItems
                .Where(w => w.MyCartUserId == user.Id)
                .Include(w => w.Movie)
                .ToListAsync();

            // Shopping Cart with expired voucher
            var shoppingCartVoucher = await _context.ShoppingVouchers
                .Include(w => w.Voucher)
                .Include(w => w.ShoppingCartItem)
                .Where(w => w.Voucher.Expire <= now || w.Voucher.Quantity <= 0)
                .ToListAsync();

            var cartToAddList = new List<ShoppingCartItem>();

            foreach (var item in shoppingCartVoucher)
            {

                var cart = allCart.FirstOrDefault(w => w.Id == item.ShoppingCartItemId);

                if (cart != null)
                {
                    var cartDiscount = item.Voucher.DiscountPercentage;
                    var cartProductPrice = cart.Movie.Price;

                    var discountedPrice = (decimal)cartProductPrice * (cartDiscount / 100m);

                    cart.Price += (double)discountedPrice;
                    cartToAddList.Add(cart);
                }

                _context.ShoppingCartItems.UpdateRange(cartToAddList);

                var shopVoucher = _context.ShoppingVouchers.FirstOrDefault(w => w.Id == item.Id);
                if (shopVoucher != null)
                    _context.ShoppingVouchers.Remove(shopVoucher);

                await _context.SaveChangesAsync();
            }*/


            // If the result is new DateTime() it means it doesnt expired yet;
            /*var doesAllValid = voucherExpires.FirstOrDefault(date => now >= date);*/



            return new MyShoppingCartItemsView()
            {
                CartListItems = cartList,
                Total= Math.Round(cartList.Select(w=> w.SelectedAmount * w.Price).Sum(), 2)
            };


        }

        
        // Adding one item to the same product in my cart.
        [HttpPut("AddAnotherSameProductToMyCart")]
        public async Task<ActionResult> AddAnotherSameProductToMyCart(int shoppingCartId)
        {
            var user = await _userManager.GetUserAsync(User);

            var productExists = await _context.ShoppingCartItems.AnyAsync(w => w.Id == shoppingCartId);
            /*var asd = await _base.AnyAsync(w=> w.Id == shoppingCartId);*/

            if (productExists == true)
            {
                // Existing specific cart
                var myCart = await _context.ShoppingCartItems
                    .Include(w=> w.Movie)
                    .Where(w => w.MyCartUserId == user.Id)
                    .FirstOrDefaultAsync(w => w.Id == shoppingCartId);

                myCart.Amount++;
                myCart.DateAddToCart = DateTime.Now;

                _context.ShoppingCartItems.Update(myCart);
                await _context.SaveChangesAsync();

                return Ok($"You have now {myCart.Amount} {myCart.Movie.Name}");

            }
            return BadRequest("Error adding");
        }
        [HttpDelete("RemoveProductToMyCart/{shoppingCartId}")]
        public async Task<ActionResult> DecreaseProductToMyCart(int shoppingCartId)
        {
            var user = await _userManager.GetUserAsync(User);

            var productExists = await _context.ShoppingCartItems.AnyAsync(w => w.Id == shoppingCartId);

            if (productExists == true)
            {
                var shoppingCart = await _context.ShoppingCartItems
                    .Include(w => w.Movie)
                    .Where(w=> w.MyCartUserId == user.Id)
                    .FirstOrDefaultAsync(w => w.Id == shoppingCartId);

                var remainingCart = shoppingCart.Amount;

                if(remainingCart >= 2)
                {
                    shoppingCart.Amount--;
                    _context.ShoppingCartItems.Update(shoppingCart);
                    await _context.SaveChangesAsync();

                    return Ok($"You have now {shoppingCart.Amount} {shoppingCart.Movie.Name}.");
                }
                else if(remainingCart <= 1)
                {
                    _context.ShoppingCartItems.Remove(shoppingCart);

                    // If exists Remove all the Message Product notification came from being stocked in cart
                    // of about couples of minutes
                    await _shoppingCart.MessageProductNotificationRemoval(user.Id);

                    return Ok($"Sucessfully removed from the cart.");
                }
                else
                    return BadRequest("No items to cart to remove.");
            }           
            return BadRequest("Error removing");
        }
 
        // Customer could add the product in the cart in single/multiple
        // Every product could have a voucher/coupon which has an absolute discount that lessen the product price
        // Or product that has a price quota that needs to exceed the price to avail the discount.
        [HttpPost("AddItemToCartMore")]
        public async Task<ActionResult> AddItemToCartMore([FromBody] AddItemToCartManyDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = new ShoppingCartItem();
            var checkMovieExists = await _context.Movies.AnyAsync(w => w.Id == dto.MovieId);

            var now = DateTime.Now;
  
            var checkProductExistsNoCouponVoucher = await _context.ShoppingCartItems
                .Where(w => w.MyCartUserId == user.Id)
                .Where(w => w.IsVoucher == false && w.IsCoupon == false)
                .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            var checkProductExistsNoVoucherWithCoupon = await _context.ShoppingCartItems
                 .Where(w => w.MyCartUserId == user.Id)
                 .Where(w => w.IsVoucher == false && w.IsCoupon == true)
                 .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            var checkProductExistsNoCouponWithVoucher = await _context.ShoppingCartItems
                   .Where(w => w.MyCartUserId == user.Id)
                   .Where(w => w.IsVoucher == true && w.IsCoupon == false)
                   .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            var checkProductExistsWithCouponWithVoucher = await _context.ShoppingCartItems
                   .Where(w => w.MyCartUserId == user.Id)
                   .Where(w => w.IsVoucher == true && w.IsCoupon == true)
                   .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            var movie = await _context.Movies
                .Include(w=> w.Vendor)
                             .FirstOrDefaultAsync(w => w.Id == dto.MovieId);
            if (checkMovieExists)
            {
                bool productOutOfStock = await ProductOutOfStock(dto);

                /*var movie = await _context.Movies
                    .Include(w=> w.Vendor)
                    .FirstOrDefaultAsync(w => w.Id == dto.MovieId);*/

                if (productOutOfStock == true)
                {
                    // Todo if the stock is now > 0, the user who attempt to shoppingCart the product 
                    // will be having a notification which are indication of product is now available.
                    // Try to prevent from coming again.
                    /*var checkIfAlreadyAttempted = await _context.MovieOutOfStocks
                        .Where(w=> w.UserAttemptToCartOutOfStockId == user.Id)
                        .AnyAsync(w => w.MovieId == movie.Id);*/

                    var checkIfAlreadyAttempted = await _shoppingCart.MovieOutOfStockUserExistsNotif(user.Id, movie.Id);

                    if (checkIfAlreadyAttempted == false)
                    {
                        await _shoppingCart.UserLoginMovieOutOfStockNotify(dto.MovieId, user, now, movie);
                        // todoo
                        return BadRequest("The stocks of selected product is now out of stock. Wait for the re-stocking. v1");
                    }
                    else
                        return BadRequest("The stocks of selected product is now out of stock. Wait for the re-stocking. v2");
                }

                var voucherList = await _context.Vouchers
                    .Where(w => w.MovieId == dto.MovieId)
                    .Where(w => w.Quantity > 0 && w.Expire > now && w.IsRemoved == false)
                    .Where(w => w.Code == null)
                    .ToListAsync();

                var couponList = await _context.Vouchers
                    .Where(w => w.MovieId == dto.MovieId)
                    .Where(w => w.Quantity > 0 && w.Expire > now && w.IsRemoved == false)
                    .Where(w => dto.VoucherCode.Contains(w.Code))
                    .ToListAsync();

                /*var movie = await _context.Movies
                             .FirstOrDefaultAsync(w => w.Id == dto.MovieId);*/

                var vendorShopQuotaMovie = await _context.DiscountedShop
                    .Where(w => w.Quantity > 0 && w.Expire >= now)
                    .Where(w => w.VendorId == movie.VendorId)
                    .ToListAsync();

                int movieShipping = movie.ShippingFee;
  
                decimal finalPrice = (decimal)movie.Price;

                decimal totalVoucherDiscount = 0;

                decimal totalCouponDiscount = 0;

                var thisMonth = now.Month;

               /* if (now.Day == thisMonth)
                {
                    // shoppee 7/7, 8/8... sale FREE SHIPPING
                    movieShipping = 0;
                }*/

                if (DateTime.IsLeapYear(now.Year) == true)
                {
                    // 15th 30th = 50 % of standard shipping fee.
                    // check the last day of feb 29
                    // the rest of month last day 30
                    if (now.Month == ShoppingCartHelper.FEBRUARY)
                        // Not leap year, the last of feb is 28\
                        // it will only compute from 15 and 28 of the month of FEBRUARY
                        movieShipping = _shoppingHall.HalfPriceShippingFee(ShoppingCartHelper.DAY15TH,
                            ShoppingCartHelper.FEBRUARY_LEAP_DAY, now, movieShipping);
                    else if (now.Month != ShoppingCartHelper.FEBRUARY)
                        // check the last day of feb 28
                        //  All of  the month EXCEPT FEBRUARY
                        // it will only compute from 15 and 30
                        movieShipping = _shoppingHall.HalfPriceShippingFee(ShoppingCartHelper.DAY15TH,
                            ShoppingCartHelper.DAY30TH, now, movieShipping);
                }
                else if (DateTime.IsLeapYear(now.Year) == false)
                {
                    if (now.Month == ShoppingCartHelper.FEBRUARY)
                        // Not leap year, the last of feb is 28\
                        // it will only compute from 15 and 28 of the month of FEBRUARY
                        movieShipping = _shoppingHall.HalfPriceShippingFee(ShoppingCartHelper.DAY15TH, ShoppingCartHelper.FEBRUARY_REGULAR_DAY,
                            now, movieShipping);
                    else
                        // check the last day of feb 28
                        if (now.Month != ShoppingCartHelper.FEBRUARY)
                        //  All of  the month EXCEPT FEBRUARY
                        // it will only compute from 15 and 30
                        movieShipping = _shoppingHall.HalfPriceShippingFee(ShoppingCartHelper.DAY15TH, ShoppingCartHelper.DAY30TH, now, movieShipping);
                }

                if (voucherList.Any() && couponList.Any())
                {
                    var voucherExpires = voucherList.Select(w => w.Expire);
                    var couponExpires = couponList.Select(w => w.Expire);
                    var expireDate = voucherExpires.Concat(couponExpires).ToList();

                    var doesAllValid = expireDate.FirstOrDefault(date => now >= date);

                    // needs all of expiration date is less than to now. else will not execute
                    // Check if the coupon and voucher is not expired.
                    if (checkProductExistsWithCouponWithVoucher != null)
                    {
                        if (doesAllValid == new DateTime())
                        {
                            /*foreach (var item in voucherList)
                            {
                                totalVoucherDiscount += item.DiscountPercentage;
                            }
                            foreach (var item in couponList)
                            {
                                totalCouponDiscount += item.DiscountPercentage;
                            }
                            decimal totalDiscount = totalVoucherDiscount + totalCouponDiscount;*/

                            decimal totalDiscount = _shoppingHall.VoucherCouponTotalDiscount(voucherList, couponList, ref totalVoucherDiscount, ref totalCouponDiscount);

                            /*finalPrice = CouponAndVoucherDiscountedPrice(movie, finalPrice,
                               totalVoucherDiscount, totalCouponDiscount);*/
                            finalPrice = _shoppingHall.CouponAndVoucherDiscountedPrice(movie, finalPrice,
                               totalVoucherDiscount, totalCouponDiscount);
                            // Rule: Price quota and Coupon is invalid to apply together.
                            // If custoemr wanted to apply the coupon (existing) but the price quota is present
                            // Coupon will be false and quota will be applied.
                            
                             
                            // There`s a existing card with voucher included.
                            // Get the user own voucher together with the cart
                            var shopeeVoucher = await _context.ShoppingVouchers
                                .Where(w => w.ShoppingVoucherUserid == user.Id)
                                .Where(w => w.ShoppingCartItemId == checkProductExistsWithCouponWithVoucher.Id)
                                .ToListAsync();

                            var voucherCoupon = new Voucher();

                            var allDiscount = voucherList.Concat(couponList).ToList();
                            int? voucherId = 0;

                            foreach (var item in shopeeVoucher)
                            {
                                // Check if voucher id and shoppingVoucherId is match
                                voucherCoupon = allDiscount.FirstOrDefault(w => w.Id == item.VoucherId);
                                voucherId = item.VoucherId;
                            }

                            if (voucherCoupon != null && voucherCoupon.Id == voucherId)
                            {
                                checkProductExistsWithCouponWithVoucher.Amount += 1;
                                _context.ShoppingCartItems.Update(checkProductExistsWithCouponWithVoucher);
                            }
                            await _context.SaveChangesAsync();

                            // If theres a similar voucherId in the DB and added another similar voucher Id
                            var isSameCartItems = shopeeVoucher.Any(w => w.ShoppingCartItemId == checkProductExistsWithCouponWithVoucher.Id);
                            if (isSameCartItems == false)
                            {
                                // Add a Shopping Voucher id that refence to the shoppingCartItem. 
                                await AddingShoppingVoucher(cart.Id, allDiscount, user.Id);
                            }
                            return Ok("Successfully purchased. With Voucher, With Coupon checkProductExistsWithCouponWithVoucher is not null ");

                        }
                        else
                        {
                            return BadRequest("Both coupon are expired.");
                        }
                    }
                    else if (checkProductExistsWithCouponWithVoucher == null)
                    {
                        if (doesAllValid == new DateTime())
                        {
                            /*if (vendorShopQuotaMovie.Count != 0)
                            {

                                return Ok("Successfully purchased a product with Voucher, with Coupon With Price Quota Create ");
                            }*/
                            /*else if (vendorShopQuotaMovie.Count == 0)
                            {*/
                            /*decimal totalDiscount = VoucherCouponTotalDiscount(voucherList, couponList, ref totalVoucherDiscount, ref totalCouponDiscount);*/
                            decimal totalDiscount = _shoppingHall.VoucherCouponTotalDiscount(voucherList, couponList, ref totalVoucherDiscount, ref totalCouponDiscount);
                            /*finalPrice = CouponAndVoucherDiscountedPrice(movie, finalPrice,
                                totalVoucherDiscount, totalCouponDiscount);*/

                            finalPrice = _shoppingHall.CouponAndVoucherDiscountedPrice(movie, finalPrice,
                               totalVoucherDiscount, totalCouponDiscount);

                            // voucher and coupon need to tracked.
                            var allDiscount = voucherList.Concat(couponList).ToList();

                            // If theres a promotion, add with promotion else not
                            if (vendorShopQuotaMovie.Count == 0 || vendorShopQuotaMovie.Count != 0)
                            {
                                // pol
                                /*cart = await AddingShoppingCartWithCouponWithVoucher(user.Id, now, movie, finalPrice, movieShipping, totalDiscount, false);*/
                                cart = await _shoppingHall.AddingShoppingCartWithCouponWithVoucher(user.Id, now, movie, finalPrice, movieShipping, totalDiscount, false);

                                /*await AddingShoppingVoucher(cart.Id, allDiscount, user.Id);*/
                                await _shoppingHall.AddingShoppingVoucher(cart.Id, allDiscount, user.Id);
                                return Ok("Successfully purchased a product with Voucher, with Coupon no Price Quota Create ");

                            }
                          
                        }
                    }
                }
                else if (voucherList.Any() == true && couponList.Any() == false)
                {
                    // If doesnt expired it will be new Datetime();
                    // if Expires, it need to remove the voucher id
                   /* var voucherExpires = voucherList.Select(w => w.Expire);*/
                    /*var doesAllValid = DoesDiscountPromotionValid(now, voucherExpires);*/
                    var doesAllValid = _shoppingHall.DoesAllDiscountValid(now, voucherList);

                    // jj
                    if (doesAllValid != new DateTime()) return BadRequest("Voucher you applied for the product is now expired");
                    // needs all of expiration date is less than to now. else will not execute
                    // Check if the coupon and voucher is not expired.
                    if (checkProductExistsNoCouponWithVoucher != null)
                    {
                        if (doesAllValid == new DateTime())
                        {
                            // If vendor suddenly make a price quota to the movie which I had in the cart
                            // to be fair, Check if IsMinimumQuota == false, If so, replaced it to true
                            if (vendorShopQuotaMovie.Count != 0)
                            {
                                checkProductExistsNoCouponWithVoucher.IsMinimumQuota = true;
                            }


                            /* finalPrice = VoucherTotalDiscountPrice(voucherList, movie, ref totalVoucherDiscount);*/
                            finalPrice = _shoppingHall.VoucherTotalDiscountPrice(voucherList, movie, ref totalVoucherDiscount);

                            // If coupon is applied and price quota is off
                            // Check the amount increase by one.


                            // There`s a existing card with voucher included.
                            // Get the user own voucher together with the cart
                            var shopeeVoucher = await _context.ShoppingVouchers
                                .Where(w => w.ShoppingVoucherUserid == user.Id)
                                .Where(w => w.ShoppingCartItemId == checkProductExistsNoCouponWithVoucher.Id)
                                .ToListAsync();

                            var voucherCoupon = new Voucher();
                            int? voucherId = 0;

                            foreach (var item in shopeeVoucher)
                            {
                                // Check if voucher id and shoppingVoucherId is match
                                voucherCoupon = voucherList.FirstOrDefault(w => w.Id == item.VoucherId);
                                voucherId = item.VoucherId;
                            }

                            if (voucherCoupon != null && voucherCoupon.Id == voucherId)
                            {
                                // popo
                                checkProductExistsNoCouponWithVoucher.Amount += 1;
                                _context.ShoppingCartItems.Update(checkProductExistsNoCouponWithVoucher);
                            }

                            await _context.SaveChangesAsync();

                            // Avoiding the similar voucherId in the DB and added another similar voucher Id
                            /*await AddingShoppingUniqueVoucher(user, cart, checkProductExistsNoCouponWithVoucher, voucherList, shopeeVoucher);*/
                            await _shoppingHall.AddingShoppingUniqueVoucher(user.Id, cart,
                                checkProductExistsNoCouponWithVoucher, voucherList, shopeeVoucher);
                            /*var isSameCartItems = shopeeVoucher.Any(w => w.ShoppingCartItemId == checkProductExistsNoCouponWithVoucher.Id);

                            if (isSameCartItems == false)
                            {
                                // Add a Shopping Voucher id that refence to the shoppingCartItem. 
                                await AddingShoppingVoucher(cart.Id, couponList, user.Id);
                            }*/
                            return Ok($"Successfully purchased. With Voucher," +
                                $" No Coupon total of {checkProductExistsNoCouponWithVoucher.Amount}");

                        }
                    }
                    // Has voucher discount but didnt have a cart before.
                    else if (checkProductExistsNoCouponWithVoucher == null)
                    {
                        if (doesAllValid == new DateTime())
                        {
                            // Check if the cart movie id have a vendor movie promotion
                            // if theres a minimum spend we need to check first the price if price > minimum
                            // if true, place order will deduct the original price with the minimun price
                            if (vendorShopQuotaMovie.Count != 0)
                            {
                               /* finalPrice = VoucherTotalDiscountPrice(voucherList, movie, ref totalVoucherDiscount);*/
                                finalPrice = _shoppingHall.VoucherTotalDiscountPrice(voucherList, movie, ref totalVoucherDiscount);

                                /*await AddingShoppingCartWithVoucherNoCouponWithQuota(user, cart, movie, finalPrice, movieShipping, totalVoucherDiscount);*/
                                // ex2
                                cart = await _shoppingHall.AddingShoppingCartWithVoucherNoCouponWithQuota(user.Id, now, movie, finalPrice,
                                    movieShipping, totalVoucherDiscount);
                                // yonko
                                /*await AddingShoppingVoucher(cart.Id, voucherList, user.Id);*/
                                await _shoppingHall.AddingShoppingVoucher(cart.Id, voucherList, user.Id);
                                /*await AddDiscountedCartQuota(user, cart, vendorShopQuotaMovie);*/
                                await _shoppingHall.AddDiscountedCartQuota(user.Id, cart, vendorShopQuotaMovie);
                                return Ok("Successfully purchased. Has a voucher no Coupon, With Price Quota create");
                            }
                            // There`s no minimum spend, so it will straightly take effect the voucher discount.
                            else if (vendorShopQuotaMovie.Count == 0)
                            {
                                /*foreach (var item in voucherList)
                                {
                                    totalVoucherDiscount += item.DiscountPercentage;
                                }
                                finalPrice = TotalDiscountedPrice(movie, totalVoucherDiscount);*/

                                /*finalPrice = VoucherTotalDiscountPrice(voucherList, movie, ref totalVoucherDiscount);*/
                                finalPrice = _shoppingHall.VoucherTotalDiscountPrice(voucherList, movie, ref totalVoucherDiscount);

                                /*await AddingShoppingCartWithVoucherNoCoupon(user, cart, movie, finalPrice, movieShipping, totalVoucherDiscount);*/
                                await _shoppingHall.AddingShoppingCartWithVoucherNoCoupon(user, cart, movie, finalPrice, movieShipping, totalVoucherDiscount);

                                /*await AddingShoppingVoucher(cart.Id, voucherList, user.Id);*/
                                await _shoppingHall.AddingShoppingVoucher(cart.Id, voucherList, user.Id);
                                return Ok("Successfully purchased. Has a voucher no Coupon, Without Price Quota create");
                            }
                        }

                    }
                }
                else if (voucherList.Any() == false && couponList.Any() == true)
                {
                    // It have a shipping fee yui
                    /*DateTime doesAllValid = DoesAllCouponValid(now, couponList);*/
                    var doesAllValid = _shoppingHall.DoesAllDiscountValid(now, couponList);

                    // Has coupon discount but did have a cart before.
                    if (checkProductExistsNoVoucherWithCoupon != null)
                    {
                        if (doesAllValid == new DateTime())
                        {
                            /*foreach (var item in couponList)
                            {
                                totalCouponDiscount += item.DiscountPercentage;
                            }

                            finalPrice = TotalDiscountedPrice(movie, totalCouponDiscount);*/

                            /*finalPrice = CouponTotalDiscountPrice(couponList, movie, ref totalCouponDiscount);*/
                            finalPrice = _shoppingHall.CouponTotalDiscountPrice(couponList, movie, ref totalCouponDiscount);

                            var shopeeVoucher = await _context.ShoppingVouchers
                                .Where(w => w.ShoppingVoucherUserid == user.Id)
                                .Where(w => w.ShoppingCartItemId == checkProductExistsNoVoucherWithCoupon.Id)
                                .ToListAsync();

                            var voucherCoupon = new Voucher();

                            foreach (var item in shopeeVoucher)
                            {
                                voucherCoupon = couponList.FirstOrDefault(w => w.Id == item.VoucherId);
                                if (voucherCoupon != null && voucherCoupon.Id == item.VoucherId)
                                {
                                    checkProductExistsNoVoucherWithCoupon.Amount += 1;
                                    _context.ShoppingCartItems.Update(checkProductExistsNoVoucherWithCoupon);
                                }
                            }
                            await _context.SaveChangesAsync();

                            // If theres a similar voucherId in the DB and added another similar voucher Id
                            // hhh
                            await AddingShoppingUniqueVoucher(user.Id, cart, checkProductExistsNoVoucherWithCoupon, couponList, shopeeVoucher);

                            /*var isSameCartItems = shopeeVoucher.Any(w => w.ShoppingCartItemId == checkProductExistsNoVoucherWithCoupon.Id);
                            if(isSameCartItems == false)
                            {
                                // Add a Shopping Voucher id that refence to the shoppingCartItem. 
                                await AddingShoppingVoucher(cart.Id, couponList, user.Id);
                            }*/

                            return Ok($"Successfully purchased. With Coupon," +
                               $" No Voucher total of {checkProductExistsNoVoucherWithCoupon.Amount}");
                        }
                    }
                    // Has coupon discount but didnt have a cart before.
                    else if (checkProductExistsNoVoucherWithCoupon == null)
                    {
                        if (doesAllValid == new DateTime())
                        {
                            /*finalPrice = CouponTotalDiscountPrice(couponList, movie, ref totalCouponDiscount);*/
                            finalPrice = _shoppingHall.CouponTotalDiscountPrice(couponList, movie, ref totalCouponDiscount);

                            /*cart = await AddingShoppingCartWithCouponNoVoucher(user.Id, now, movie, finalPrice, movieShipping, totalCouponDiscount);*/
                            // ex1
                            cart = await _shoppingHall.AddingShoppingCartWithCouponNoVoucher(user.Id, now, movie, finalPrice, movieShipping, totalCouponDiscount);

                            /*await AddingShoppingVoucher(cart.Id, couponList, user.Id);*/
                            await _shoppingHall.AddingShoppingVoucher(cart.Id, couponList, user.Id);

                            return Ok("Successfully purchased. Has a Coupon no Voucher, COULD`NT BE HAVE QUOTA create");
                        }
                        else
                            return BadRequest("Both coupon are expired.");
                    }
                }
                // No coupon no voucher
                else if (voucherList.Any() == false && couponList.Any() == false)
                {
                    // No coupon no voucher cart doesnt empty
                    if (checkProductExistsNoCouponVoucher != null )
                    {

                        if (vendorShopQuotaMovie.Count != 0)
                        {
                            // do it to otherse end point
                            /*await NoCouponWithVoucherAndQuotaIncreaseAmount(dto, user);*/
                            await _shoppingHall.NoCouponWithVoucherAndQuotaIncreaseAmount(dto, user);

                            /*var checkProductExistsNoCouponVoucherWithQuota = await _context.ShoppingCartItems
                               .Where(w => w.MyCartUserId == user.Id)
                               .Where(w => w.IsVoucher == false && w.IsCoupon == false)
                               .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

                            checkProductExistsNoCouponVoucherWithQuota.Amount += 1;
                            _context.ShoppingCartItems.Update(checkProductExistsNoCouponVoucherWithQuota);
                            await _context.SaveChangesAsync();*/
                            return Ok("Successfully purchased. No coupon, No Voucher With Quota plus one amount ");
                        }
                        else if (vendorShopQuotaMovie.Count == 0)
                        {
                            // No movie quota No coupon no voucher 
                            checkProductExistsNoCouponVoucher.Amount += 1;
                            _context.ShoppingCartItems.Update(checkProductExistsNoCouponVoucher);

                            await _context.SaveChangesAsync();
                            return Ok("Successfully purchased. No coupon, No Voucher  plus one amount");

                        }


                    }
                    // No coupon no voucher cart is empty
                    else if (checkProductExistsNoCouponVoucher == null )
                    {
                        if (vendorShopQuotaMovie.Count != 0)
                        {
                            
                            /*await AddingShoppingCartNoCouponNoVoucherWithQuota(user, cart, movie, finalPrice, movieShipping);*/
                            await _shoppingHall.AddingShoppingCartNoCouponNoVoucherWithQuota(user, cart, movie, finalPrice, movieShipping);
                            // Voucher, Coupon but the movie product has a vendor promotion.

                            /*await AddDiscountedCartQuota(user, cart, vendorShopQuotaMovie);*/
                            await _shoppingHall.AddDiscountedCartQuota(user.Id, cart, vendorShopQuotaMovie);
                            return Ok("Successfully purchased. No coupon, No Voucher, With Movie Price Quota create");
                        }
                        else if (vendorShopQuotaMovie.Count == 0)
                        {
                            /*await AddingShoppingCartNoCouponNoVoucher(user, cart, movie, finalPrice, movieShipping);*/

                            await _shoppingHall.AddingShoppingCartNoCouponNoVoucher(user, cart, movie, finalPrice, movieShipping);

                            // lll
                            /*cart = await AddingShoppingCart(user, now, movie, finalPrice);*/
                            return Ok("Successfully purchased. No coupon, No Voucher checkProductExistsNoCouponVoucher is null ");
                        }

                        
                    }
                }
      
                /*if (DateTime.IsLeapYear(now.Year) == true)
                {       
                    movieShipping = LeapYearFee(now, movieShipping, FEBRUARY, 29, 30);  
                }
                else if (DateTime.IsLeapYear(now.Year) == false)
                {
                    movieShipping = LeapYearFee(now, movieShipping, 8, 3, 3);
                }*/
            }

            return Ok();
        }

        private async Task UserLoginMovieOutOfStockNotify(int movieId, ApplicationUser user, DateTime now, Movie movie)
        {
            var movieOutOfStock = new MovieOutOfStock()
            {
                UserAttemptToCartOutOfStockId = user.Id,
                DateCreation = now,
                MovieId = movieId,
                IsOutOfStock = true
            };
            await _context.MovieOutOfStocks.AddAsync(movieOutOfStock);
            await _context.SaveChangesAsync();

            var movieOutOfStockNotification = new MovieOutOfStockNotification()
            {
                ReceivingUserId = user.Id,
                MovieOutOfStockId = movieOutOfStock.Id,
                Creation = now,
                Header = $"Good Day {user.UserName}! The product you`ve selected is now out of stock.",
                MessageBody = $"The {movie.Name} from {movie.Vendor.UserName} is currently out of stock. We had already notified him to re-stock your selected product. ",
                ProductImage = movie?.ImageURL
            };
            await _context.MovieOutOfStockNotifications.AddAsync(movieOutOfStockNotification);
            await _context.SaveChangesAsync();
        }

        private static decimal VoucherCouponTotalDiscount(List<Voucher> voucherList, List<Voucher> couponList, ref decimal totalVoucherDiscount, ref decimal totalCouponDiscount)
        {
            foreach (var item in voucherList)
            {
                totalVoucherDiscount += item.DiscountPercentage;
            }
            foreach (var item in couponList)
            {
                totalCouponDiscount += item.DiscountPercentage;
            }

            decimal totalDiscount = totalVoucherDiscount + totalCouponDiscount;
            return totalDiscount;
        }

        private static DateTime DoesAllCouponValid(DateTime now, List<Voucher> couponList)
        {
            var couponExpires = couponList.Select(w => w.Expire);
            // If the result is new DateTime() it means it doesnt expired yet;
            var doesAllValid = DoesDiscountPromotionValid(now, couponExpires);
            return doesAllValid;
        }
        private async Task NoCouponWithVoucherAndQuotaIncreaseAmount(AddItemToCartManyDTO dto, ApplicationUser user)
        {
            var checkProductExistsNoCouponVoucherWithQuota = await _context.ShoppingCartItems
                                            .Where(w => w.MyCartUserId == user.Id)
                                            .Where(w => w.IsVoucher == false && w.IsCoupon == false)
                                            .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            checkProductExistsNoCouponVoucherWithQuota.Amount += 1;
            _context.ShoppingCartItems.Update(checkProductExistsNoCouponVoucherWithQuota);
            await _context.SaveChangesAsync();
        }

        [HttpPut("ChangeDiscount")]
        public async Task<ActionResult> ChangeDiscount([FromBody] AddItemToCartManyDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);

            await _shoppingCart.PutChangeDiscount(user.Id, dto);

            //

            /*var movie = await _context.Movies
                            .FirstOrDefaultAsync(w => w.Id == dto.MovieId);

            var now = DateTime.Now;

            var vendorShopQuotaMovie = await _context.DiscountedShop
                    .Where(w => w.Quantity > 0 && w.Expire >= now)
                    .Where(w => w.VendorId == movie.VendorId)
                    .ToListAsync();

            var cart = new ShoppingCartItem();

            // Error discount changing in the view // Fix
            var voucherWithoutCouponCart = await _context.ShoppingCartItems
                              .Where(w => w.MyCartUserId == user.Id)
                              .Where(w => w.IsVoucher == true && w.IsCoupon == false)
                              .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            if (voucherWithoutCouponCart == null) return BadRequest("You had already coupon!!!");

                var couponWithVoucherCart = await _context.ShoppingCartItems
                              .Where(w => w.MyCartUserId == user.Id)
                              .Where(w => w.IsVoucher == true && w.IsCoupon == true)
                              .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            var couponExists = await _context.Vouchers
                   .Where(w => w.Quantity > 0 || w.Expire > now)
                   .FirstOrDefaultAsync(w => dto.VoucherCode.Contains(w.Code));

            var couponList = await _context.Vouchers
              .Where(w => w.Quantity > 0 || w.Expire > now)
              .Where(w => dto.VoucherCode.Contains(w.Code))
              .ToListAsync();

            var couponValueCart = await _context.Vouchers
                   .Where(w => w.Quantity > 0 || w.Expire > now)
                   .Where(w=> w.Code != null)
                   .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            var discountedCart = new DiscountShop_Cart();

            if (voucherWithoutCouponCart != null)
            {
                discountedCart = _context.DiscountShop_Cart
                   .Include(w => w.DiscountedShop)
                   .Where(w => w.DiscountedShop.VendorId == voucherWithoutCouponCart.VendorId)
                   .FirstOrDefault(w => w.ShoppingCartItemId == voucherWithoutCouponCart.Id);
            }
            // Get the Coupon to remove.

            var shoppingVoucher = new ShoppingVoucher();

           *//* var voucherCodeId = await _context.Vouchers
                .Where(w => w.Code != null)
                .FirstOrDefaultAsync(w => w.MovieId == couponWithVoucherCart.MovieId);*//*

            if (couponWithVoucherCart != null)
            {
                shoppingVoucher = await _context.ShoppingVouchers
                    .Include(w=> w.Voucher)
                    .Where(w => w.ShoppingVoucherUserid == user.Id)
                    .Where(w=> w.Voucher.Code != null)
                    .FirstOrDefaultAsync(w => w.ShoppingCartItemId == couponWithVoucherCart.Id);

            }

            if (vendorShopQuotaMovie != null || vendorShopQuotaMovie.Count != 0)
            {
                if (couponExists != null && discountedCart != null)
                {
                    *//*voucherWithoutCouponCart.IsCoupon = true;
                    voucherWithoutCouponCart.TotalDiscount += couponExists.DiscountPercentage;
                    voucherWithoutCouponCart.IsMinimumQuota = false;*//*

                    PriceQuotaAndCouponAdjustment(true, true, false, voucherWithoutCouponCart, couponExists);

                    if(voucherWithoutCouponCart != null || couponList != null)
                    {
                        await AddingShoppingVoucher(voucherWithoutCouponCart.Id, couponList, user.Id);

                    }

                    if (discountedCart != null)
                        // remove the quota discount cart.
                        _context.DiscountShop_Cart.Remove(discountedCart);
                    
                    await _context.SaveChangesAsync();
                    return Ok("Successfully edit your cart. Coupon is now applied but Price quota is dismantle");

                }
                else if (couponExists == null && couponWithVoucherCart != null)
                {
                    *//*couponWithVoucherCart.IsCoupon = false;
                    couponWithVoucherCart.TotalDiscount -= couponValueCart.DiscountPercentage;
                    couponWithVoucherCart.IsMinimumQuota = true;*//*

                    PriceQuotaAndCouponAdjustment(false, false, true, couponWithVoucherCart, couponValueCart);

                    await AddDiscountedCartQuota(user, couponWithVoucherCart, vendorShopQuotaMovie);

                    if (shoppingVoucher != null)
                        // remove the shopee voucher
                        _context.ShoppingVouchers.Remove(shoppingVoucher);

                    await _context.SaveChangesAsync();
                    return Ok("Successfully edit your cart. Price quota is now applied but Coupon is off");
                }
            }*/
            /*return BadRequest("Error discount changing");*/
            return Ok();
        }
        private static void PriceQuotaAndCouponAdjustment(
            bool IsCoupon, bool IsTotalDiscountAdd, bool IsMinimumQuota,
           ShoppingCartItem couponWithVoucherCart, Voucher couponValueCart)
        {
            if (couponWithVoucherCart == null) return;

            if(couponWithVoucherCart != null && IsCoupon == true)
            {
                couponWithVoucherCart.IsCoupon = true;
            }
            if (couponWithVoucherCart != null &&IsCoupon == false)
            {
                couponWithVoucherCart.IsCoupon = false;
            }
            if (IsTotalDiscountAdd == true)
            {
                couponWithVoucherCart.TotalDiscount += couponValueCart.DiscountPercentage;
            }
            if (IsTotalDiscountAdd == false)
            {
                couponWithVoucherCart.TotalDiscount -= couponValueCart.DiscountPercentage;

            }
            if (IsMinimumQuota == true)
            {
                couponWithVoucherCart.IsMinimumQuota = true;
            }
            if (IsMinimumQuota == false)
            {
                couponWithVoucherCart.IsMinimumQuota = false;

            }

            /*voucherWithoutCouponCart.IsCoupon = true;
            voucherWithoutCouponCart.TotalDiscount += couponExists.DiscountPercentage;
            voucherWithoutCouponCart.IsMinimumQuota = false;*/

            /*couponWithVoucherCart.IsCoupon = false;
            couponWithVoucherCart.TotalDiscount -= couponValueCart.DiscountPercentage;
            couponWithVoucherCart.IsMinimumQuota = true;*/

            /*couponWithVoucherCart.IsCoupon = false;
            couponWithVoucherCart.TotalDiscount -= couponValueCart.DiscountPercentage;
            couponWithVoucherCart.IsMinimumQuota = true;*/

        }
 
        private static DateTime DoesDiscountPromotionValid(DateTime now, IEnumerable<DateTime> voucherExpires)
        {
            return voucherExpires.FirstOrDefault(expireDate => now > expireDate);
        }

        private async Task AddingShoppingUniqueVoucher(string userId, ShoppingCartItem cart,
            ShoppingCartItem checkProductExistsNoCouponWithVoucher, List<Voucher> voucherList, List<ShoppingVoucher> shopeeVoucher)
        {
            var isSameCartItems = shopeeVoucher.Any(w => w.ShoppingCartItemId == checkProductExistsNoCouponWithVoucher.Id);

            if (isSameCartItems == false)
            {
                // Add a Shopping Voucher id that refence to the shoppingCartItem. 
                await AddingShoppingVoucher(cart.Id, voucherList, userId);
            }
        }
 
        
        private async Task<bool> ProductOutOfStock(AddItemToCartManyDTO dto)
        {
            return await _context.Movies
                    .Where(w => w.StockCount == 0)
                    .AnyAsync(w => w.Id == dto.MovieId);
        }
 
        // Create an generate of this.
        private async Task AddingShoppingVoucher(int cartId, List<Voucher> voucherDiscountList, string userId)
        {

            var shopVoucher = (from p in voucherDiscountList
                               select new ShoppingVoucher()
                               {
                                   ShoppingCartItemId = cartId,
                                   VoucherId = p.Id,
                                   ShoppingVoucherUserid = userId
                               }).ToList();

            await _context.ShoppingVouchers.AddRangeAsync(shopVoucher);
            await _context.SaveChangesAsync();
        }
       
        /*private int GetMonthInDays()
        {
            DateTime startUpTime = DateTime.Now;

            return startUpTime.Month switch
            {
                1 => 31,
                2 => startUpTime.Year % 4 == 0 ? 29 : 28,
                 
                    
            };

        }*/
    }
}
