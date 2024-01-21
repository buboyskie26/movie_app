using API.Strategy.PlaceOrder.DiscountExpiration;
using Application.Helper;
using Application.Repository.Factory.PlaceOrder;
using Application.Repository.IService;
using Application.ViewModel.Movie;
using Application.ViewModel.PlaceOrder;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlaceOrderController : ControllerBase
    {       
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService fileStorageService;
        private string container = "order";
        private readonly IShoppingCart _shoppingCart;
        private readonly IPlaceOrder _placeOrder;

        public PlaceOrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IFileStorageService fileStorageService,
            IShoppingCart shoppingCart, IPlaceOrder placeOrder)
        {
            _context = context;
            _userManager = userManager;
            this.fileStorageService = fileStorageService;
            _shoppingCart = shoppingCart;
            _placeOrder = placeOrder;
        }

        //
        [HttpPost("PlacedAllMyOrder")]
        public async Task<ActionResult> PlacedSingleOrderMany([FromForm] MovieCheckout dto)
        {

            var user = await _userManager.GetUserAsync(User);

            string VENDOR_SHIP_THE_PARCEL = "Vendor is preparing to ship your parcel";

            var itemToChange = new Movie();
            var itemToChanges = new List<Movie>();

            var now = DateTime.Now;

            var checkCartExists = await _context.ShoppingCartItems
                .AnyAsync(w => dto.ShoppingCartId.Contains(w.Id));

            var response = new ProductTransactionResponse();
            var placingOrder = new PlaceOrderItems();

            /*if (dto.ShoppingCartId is string)
                return BadRequest("qwe");*/

            if (checkCartExists == true)
            {
                var cartItems = await _context.ShoppingCartItems
                    .FirstOrDefaultAsync(w => dto.ShoppingCartId.Contains(w.Id));

                bool productOutOfStock = await _context.Movies
                                           .Where(w => w.StockCount == 0)
                                           .AnyAsync(w => cartItems.MovieId == w.Id);

                /*if (productOutOfStock == true)
                {
                    // Todo if the stock is now > 0, the user who attempt to shoppingCart the product 
                    // will be having a notification indication of product is now available.
                    return BadRequest("The stocks of selected product is now out of stock. Wait for the re-stocking.");
                }*/

                var myFinalCartWithoutQuota = await _placeOrder.MyShoppingCartList(dto, user.Id, false);
                var myFinalCartWithQuota = await _placeOrder.MyShoppingCartList(dto, user.Id, true);
                
                if (myFinalCartWithoutQuota != null)
                {
                    var placeOrder = (from t in myFinalCartWithoutQuota
                                      select new PlaceOrderItems
                                      {
                                          Amount = t.Amount,
                                          Price = t.Price,
                                          UserPlaceOrderId = user.Id,
                                          PlacedOrderCreation = now,
                                          MovieId = t.MovieId,
                                          VendorId = t.VendorId,
                                          TotalPrice = (t.Amount * t.Price) + t.ShippingFee,
                                          ShippingFee = t.ShippingFee,
                                          TotalDiscount = t.TotalDiscount
                                      }).ToList();



                    /*quotaCart = myFinalCartWithQuota.FirstOrDefault(w => dto.ShoppingCartId.Contains(w.Id));
                    quotaCartList.Add(quotaCart);*/

                    var discountedExpiresCartList = await _context.DiscountShop_Cart
                        .Include(w => w.DiscountedShop)
                        .Where(w => w.DiscountedShop.Expire <= now || w.DiscountedShop.Quantity == 0)
                        .Where(w => w.ShoppingVoucherUserid == user.Id)
                        .ToListAsync();

                    var quotaCart = new ShoppingCartItem();
                    var quotaCartList = new List<ShoppingCartItem>();

                    var vendorExpiresPromotion = new DiscountedShop();

                    foreach (var item in discountedExpiresCartList)
                    {
                        quotaCart = myFinalCartWithQuota.FirstOrDefault(w => w.Id == item.ShoppingCartItemId);
                        if (quotaCart != null && quotaCart.Id != 0)
                        {
                            quotaCartList.Add(quotaCart);
                        }
                    }
                    if (quotaCartList.Count != 0)
                    {
                        // Note: Deletion of all vendor promotion if it was expired.
                        var expirePromotionShop = _context.DiscountedShop
                            .Where(w => w.Expire <= now || w.Quantity == 0)
                            .ToList();
                        /*_context.DiscountedShop.RemoveRange(expirePromotionShop);*/

                        // Update the IsMinimummQuota to false and delete the vendor promotion minimum spend.
                        _context.AttachRange(myFinalCartWithQuota);
                        // The specific vendor promotion is now expired,
                        // The applied promotion to the product will be now void.
                        myFinalCartWithQuota.ForEach(w =>
                        {
                            w.IsMinimumQuota = false;
                        });
                        await _context.SaveChangesAsync();
                        return BadRequest("The promotion of the vendor is now expires. Please review your cart");
                    }

                    // Check if the Selected amount is greater than the movie stock if so
                    // return badrequest and changed the 


                    // Check if the cart movieId have a similar movieId in the discount(coupon or voucher)
                    // if not remove the shoppeVoucher, IsCoupon changed into false, totalDiscount alignment.

                    var shoppingCouponCart = await _context.ShoppingCartItems
                        .Where(w => dto.ShoppingCartId.Contains(w.Id))
                        .Where(w => w.MyCartUserId == user.Id && w.IsCoupon == true)
                        .ToListAsync();

                    var shoppingVoucherCart = await _context.ShoppingCartItems
                       .Where(w => dto.ShoppingCartId.Contains(w.Id))
                       .Where(w => w.MyCartUserId == user.Id && w.IsVoucher == true)
                       .ToListAsync();

                    var checkCouponMovie = _context.Vouchers
                        .Where(w => w.Code != null)
                        .Where(w => shoppingCouponCart.Select(w => w.MovieId).Contains(w.MovieId))
                        .Where(w => w.IsRemoved == true)
                        .ToList();

                    var checkVoucherMovie = _context.Vouchers
                        .Where(w => shoppingVoucherCart.Select(w => w.MovieId).Contains(w.MovieId))
                        .Where(w => w.Code == null && w.IsRemoved == true)
                        .ToList();

                    int voucherxId = 0;
                    int voucherDiscount = 0;
                    int couponDiscount = 0;
                    int couponId = 0;

                    var couponOperation = new OperationCoupon();

                    var (couponExpires, voucherExpires) = couponOperation.CouponChoices(checkCouponMovie != null,
                        checkCouponMovie.Count != 0, checkVoucherMovie.Count == 0,
                        checkVoucherMovie != null,
                        checkVoucherMovie.Count != 0, checkCouponMovie.Count == 0,
                        _placeOrder);

                    // Polymorphism of CouponMovieRefresher && VoucherMovieRefresher
                    /*if (couponExpires)
                    {
                        await couponOperation.TriggerOperation(checkCouponMovie, couponId,
                           couponDiscount, shoppingCouponCart);
                        return BadRequest("Coupon is expires, Your Cart has been refresh. Check the details before you order.");
                    }
                    else if (voucherExpires)
                    {
                        await couponOperation.TriggerOperation(checkVoucherMovie, voucherxId,
                             voucherDiscount, shoppingVoucherCart);
                        return BadRequest("Voucher was expires, Your Cart has been refresh. Check the details before you order.");
                    }*/

                    //
                    if (checkCouponMovie != null && checkCouponMovie.Count != 0
                        && checkVoucherMovie.Count == 0)
                    {
                        await _placeOrder.CouponMovieRefresher(checkCouponMovie, couponId,
                            couponDiscount, shoppingCouponCart);

                        return BadRequest("Coupon is expires, Your Cart has been refresh. Check the details before you order.");
                    }
                    else if (checkVoucherMovie != null && checkVoucherMovie.Count != 0
                         && checkCouponMovie.Count == 0)
                    {
                        await _placeOrder.VoucherMovieRefresher(checkVoucherMovie, voucherxId, voucherDiscount,
                            shoppingVoucherCart);
                        /*await _placeOrder.DiscountMovieRefresher(checkVoucherMovie, voucherxId, voucherDiscount, shoppingVoucherCart, true);*/
                        return BadRequest("Voucher was expires, Your Cart has been refresh. Check the details before you order.");
                    }
                    else if (checkVoucherMovie.Count != 0 && checkCouponMovie.Count != 0)
                    {
                        await _placeOrder.CouponMovieRefresher(checkCouponMovie, couponId, couponDiscount, shoppingCouponCart);
                        await _placeOrder.VoucherMovieRefresher(checkVoucherMovie, voucherxId, voucherDiscount, shoppingVoucherCart);

                        return BadRequest("Coupon && Voucher is now expires, Your Cart has been refresh. Check the details before you order.");
                    }

                    // NOTE: Voucher Expires Cart Refresher.
                    var shopeeVoucherEject = new ShoppingVoucher();

                    // First before going to placed an order.
                    // Check if the quantity is available or > 0
                    // else the user who wanted to placed an order which they had a remaining (bug)none voucher
                    // their shoppingCart price needs to be refreshed to make the price would be accurate..

                    var voucherNotAvailable = _context.Vouchers
                            .Where(w => w.Quantity == 0 || w.Expire < now).ToList();

                    var voucherReject = new Voucher();
                    var cartRejectList = new List<ShoppingCartItem>();

                    int? voucherId = 0;
                    var cartReject = new ShoppingCartItem();

                    var shopeeInvalidVoucher = await _context.ShoppingVouchers
                            .Where(w => w.Voucher.Quantity == 0 || w.Voucher.Expire <= now)
                            .Where(w => w.ShoppingVoucherUserid == user.Id)
                            .Where(w => dto.ShoppingCartId.Contains(w.ShoppingCartItemId))
                            .ToListAsync();

                    // Loop all invalid vouchers.
                    foreach (var item in shopeeInvalidVoucher)
                    {
                        voucherReject = voucherNotAvailable.FirstOrDefault(w => w.Id == item.VoucherId);

                        cartReject = myFinalCartWithQuota.FirstOrDefault(w => w.Id == item.ShoppingCartItemId);

                        if (voucherReject != null)
                        {
                            shopeeVoucherEject = shopeeInvalidVoucher
                                 .FirstOrDefault(w => w.VoucherId == voucherReject?.Id);

                            var shoppingCartList = await _context.ShoppingCartItems
                                .Include(w => w.Movie)
                                .Include(w => w.Movie).ThenInclude(w => w.Vendor)
                                .Where(w => w.MyCartUserId == user.Id)
                                .Where(w => dto.ShoppingCartId.Contains(w.Id))
                                .ToListAsync();

                            cartReject = shoppingCartList?.FirstOrDefault(w => w.Id == shopeeVoucherEject?.ShoppingCartItemId);
                        }

                        voucherId = item.VoucherId;
                        if (cartReject.Id != 0 && cartReject != null)
                        {
                            cartRejectList.Add(cartReject);

                        }
                    }
                    if (cartRejectList.Count != 0 && voucherReject.Id == voucherId)
                    {
                        // This point. when the your cart had (discount)voucher but it was now zero
                        // It will update the true price of that cart
                        // To ensure the shoppingCart price is accurate
                        await _shoppingCart.RefreshCart(user.Id, dto.ShoppingCartId);
                        // Voucher by the user must be IsVoucher == false, because it was expired.
                        // TotalDiscount must decrease according to its value
                        // The Placing order must be stop that has a voucher expires.
                        return BadRequest($"Oops. Product voucher seemed to be expired. Check again your updated cart");
                    }

                  

                    /*var placementOrder = new PlaceOrderItems();
                    var placementOrderList = new List<PlaceOrderItems>();*/

                    // All shoppingCart With Price Quota, 
                    // Condition
                    // Ordering item that is equal to the available product
                    // Ordering item too much to the available product
                    // Ordering not equal to the available product but it left some available product.
                    var shoppingCartSelectedMovieId = _context.ShoppingCartItems
                        .Where(w => dto.ShoppingCartId.Contains(w.Id))
                        .Where(w => w.MyCartUserId == user.Id)
                        .Select(w => w.MovieId)
                        .ToList();

                    var movieSelectedList = await _context.Movies
                        .Where(w => shoppingCartSelectedMovieId.Contains(w.Id))
                        .ToListAsync();


                    var movieSelected = new Movie();

                    var placementWithQuotaOrder = new PlaceOrderItems();
                    var placementWithQuotaOrderList = new List<PlaceOrderItems>();


                    /*var shopDiscountedList = _context.DiscountShop_Cart
                           .Include(w => w.DiscountedShop)
                           .ToList();*/


                    if (myFinalCartWithQuota.Count != 0)
                    {
                        double priceQuota = myFinalCartWithQuota.Select(w => w.Price).Sum();
                        double amountQuota = myFinalCartWithQuota.Select(w => w.Amount).Sum();
                        /*var shopDiscount = new DiscountShop_Cart();*/

                        var shopDiscountedList =  _placeOrder.ShopDiscountedList();

                        // If movieSelectedList is (2), myFinalCartWithQuota needs to be (2)
                        foreach (var item in myFinalCartWithQuota)
                        {
                            /*shopDiscount = shopDiscountedList.FirstOrDefault(w => w.DiscountedShop.VendorId == item.Movie.VendorId);*/
                            var movieVendorId = item.Movie.VendorId;
                            var shopDiscount = _placeOrder.ShopDiscount(movieVendorId);

                            movieSelected = movieSelectedList.FirstOrDefault(w => w.Id == item.MovieId);

                            if (shopDiscount != null)
                            {
                                // We have an shortage in the amount of selected product.
                                var excessAmount = Math.Abs(item.Amount - movieSelected.StockCount);
                                int validAmount = item.Amount - excessAmount;
                                var totalPrice = priceQuota * amountQuota;
                                // If item.Amount > movieSelected.StockCount is true, validAmount * item.Price is applied
                                // Else If item.Amount <= movieSelected.StockCount is true, item.Amount * item.Price is applied
                                // Else -1
                                double reachedPriceQuota = _placeOrder.ReachedPriceQuota(movieSelected, item, validAmount);

                                // Customer some  product amount is bought (stock shortage).
                                if (item.Amount > movieSelected.StockCount)
                                {
                                    // TODO: In the ShoppnigCart Amount should decreased by validAmount. Remove selected Cart
                                    if (_placeOrder.ReachedPriceQuotaMinimumSpend(reachedPriceQuota, movieVendorId))
                                    {
                                        var (getAllPercentage, totalOriginalPrice) = _placeOrder.GeneratedReachedPriceQuota(shopDiscount, item, validAmount);

                                        placementWithQuotaOrder = _placeOrder.AddValidAmountOrder(user, now, placementWithQuotaOrderList,
                                            item, validAmount, getAllPercentage, totalOriginalPrice);
                                    }
                                    // TODO: In the ShoppnigCart Amount should decreased by validAmount.
                                    else if (reachedPriceQuota <= shopDiscount.DiscountedShop.MinimumSpend)
                                    {

                                        var (getAllPercentage, totalOriginalPrice) = _placeOrder.GeneratedNonReachedPriceQuota(item, validAmount);

                                        placementWithQuotaOrder = _placeOrder.AddValidAmountOrder(user, now, placementWithQuotaOrderList,
                                            item, validAmount, getAllPercentage, totalOriginalPrice);

                                    }
                                }
                                // Customer all product amount is bought TODO: Remove all used shopping Cart.
                                // We dont have shortage of product stock count.
                                else if (item.Amount <= movieSelected.StockCount)
                                {
                                    if (reachedPriceQuota >= shopDiscount.DiscountedShop.MinimumSpend)
                                    {
                                        // abhh
                                        var (getAllPercentage, totalOriginalPrice) = _placeOrder.GeneratedReachedPriceQuota(shopDiscount, item, item.Amount);

                                        placementWithQuotaOrder = _placeOrder.AddWholeAmountOrder(user.Id, now, placementWithQuotaOrderList,
                                          item, getAllPercentage, totalOriginalPrice);

                                    }
                                    else if (reachedPriceQuota <= shopDiscount.DiscountedShop.MinimumSpend)
                                    {
                                        var (getAllPercentage, totalOriginalPrice) = _placeOrder.GeneratedNonReachedPriceQuota(item, item.Amount);

                                        placementWithQuotaOrder = _placeOrder.AddWholeAmountOrder(user.Id, now, placementWithQuotaOrderList,
                                            item, getAllPercentage, totalOriginalPrice);
                                    }
                                }
                            }
                        }
                    }

                    var placementWithoutQuotaOrder = new PlaceOrderItems();
                    var placementWithoutQuotaOrderList = new List<PlaceOrderItems>();
                    var movieSelectedNonQuota = new Movie();

                    // Next todo
                    if (myFinalCartWithoutQuota.Count != 0)
                    {
                        foreach (var item in myFinalCartWithoutQuota)
                        {
                            movieSelectedNonQuota = movieSelectedList.FirstOrDefault(w => w.Id == item.MovieId);

                            var excessAmount = Math.Abs(item.Amount - movieSelectedNonQuota.StockCount);
                            int validAmount = item.Amount - excessAmount;

                            // TODO: In the ShoppnigCart Amount should decreased by validAmount. Remove selected Cart
                            // No quota but still have a voucher discount

                            if (item.Amount > movieSelectedNonQuota.StockCount)
                            {

                                var (getAllPercentage, totalOriginalPrice) = _placeOrder.GeneratedReachedWithoutPriceQuota(item, validAmount);
                                
                                placementWithoutQuotaOrder = _placeOrder.AddValidAmountWithoutQuotaOrder(user.Id,
                                    now, placementWithoutQuotaOrderList, item, validAmount,
                                    getAllPercentage, totalOriginalPrice);
                                 
                            }
                            // Product purchased that left available product
                            else if (item.Amount <= movieSelectedNonQuota.StockCount)
                            {

                                var (getAllPercentage, totalOriginalPrice) = _placeOrder.GeneratedNonReachedWithoutPriceQuota(item, item.Amount);

                            
                                placementWithoutQuotaOrder = _placeOrder.AddWholeAmountWithoutQuotaOrder(user.Id,
                                    now, placementWithoutQuotaOrderList, item, getAllPercentage, totalOriginalPrice);

                            }
                        }
                        
                    }

                    /* var withAndNonQuotaPlaceOrderList = new List<PlaceOrderItems>();*/

                    // Handles the myFinalCartWithQuotaCount and myFinalCartWithoutQuotaCount
                    // Of adding an PlaceOrderItems
                    await _placeOrder.HandlingCartWithOrWithoutQuota(user, VENDOR_SHIP_THE_PARCEL,
                        now, myFinalCartWithoutQuota.Count, myFinalCartWithQuota.Count,
                        placementWithQuotaOrderList, placementWithoutQuotaOrderList);

                    var movieSelectedNonAndWithQuota = new Movie();
                    // Movie Deletion Price Quota.
                    // For Exceeding Shopping Cart
                    // Loop only the shoppingCart you selected to avoid unnecessary movie looping.
                    var movieToChange = new Movie();
                    var movieToChangeList = new List<Movie>();
                    int MINIMUM_STOCK_COUNT = 0;

                    var cartWithQuotaToAdd = new List<ShoppingCartItem>();
                    var cartWithQuotaToRemove = new List<ShoppingCartItem>();

                    if (myFinalCartWithQuota.Count != 0)
                    {
                        foreach (var item in myFinalCartWithQuota)
                        {
                            //  decrease the amount of stock and adding the sold count in movie
                            movieToChange = movieSelectedList.FirstOrDefault(w => w.Id == item.MovieId);

                            if (movieToChange != null)
                            {
                                if (movieToChange.StockCount > 0)
                                {
                                    if (movieToChange.StockCount - item.Amount < MINIMUM_STOCK_COUNT)
                                    {
                                        var excesssProductNumber = Math.Abs(movieToChange.StockCount - item.Amount);
                                        var exactAmount = item.Amount - excesssProductNumber;

                                        var StockCountResult = item.Amount - excesssProductNumber;

                                        /*movieToChange.Sold += exactAmount;
                                        movieToChange.StockCount = movieToChange.StockCount - exactAmount;
                                        movieToChangeList.Add(movieToChange);*/

                                        // ilil
                                        _placeOrder.AddingMovieToChangeWithQuotav2(movieToChange,
                                          movieToChangeList, exactAmount, StockCountResult);
                                        
                                        item.Amount -= exactAmount;
                                        cartWithQuotaToAdd.Add(item);
                                    }
                                    else if (movieToChange.StockCount - item.Amount >= MINIMUM_STOCK_COUNT)
                                    {
                                        /*movieToChange.Sold += item.Amount;
                                        movieToChange.StockCount -= item.Amount;
                                        movieToChangeList.Add(movieToChange);*/

                                        _placeOrder.AddingMovieToChangeWithQuota(movieToChange,
                                            movieToChangeList, item.Amount, item.Amount);
                                        cartWithQuotaToRemove.Add(item);

                                    }
                                }
                                else if (movieToChange.StockCount == 0)
                                {
                                    var shoppingCart = _context.ShoppingCartItems
                                        .Where(w => w.MyCartUserId == user.Id)
                                        .Where(w=> dto.ShoppingCartId.Contains(w.Id))
                                        .ToList();

                                    // error 1
                                    var checkIfAlreadyAttempted = await _placeOrder.MovieOutOfStockUserExistsNotif(user.Id, 
                                        shoppingCart.Select(w=> w.MovieId).ToList());

                                    bool userExistsInMovieStock = await _placeOrder.UserExistsInMovieOutOfStock(user, dto);
 
                                    if (userExistsInMovieStock == false)
                                    {
                                        await _placeOrder.UserLoginMovieOutOfStockNotifyList(user,dto, now);
                                        // todoo
                                        return BadRequest("The stocks of selected product is now out of stock. Wait for the re-stocking. v1");
                                    }
                                    else if (userExistsInMovieStock == true)
                                        return BadRequest("The stocks of selected product is now out of stock. Wait for the re-stocking. v2");

                                    /* return BadRequest("Movie Product is now out of stock.");*/

                                }
                            }
                        }

                        if (movieToChangeList != null && movieToChangeList.Count != 0
                            && cartWithQuotaToAdd.Count != 0 && cartWithQuotaToRemove.Count == 0)
                        {
                            _context.Movies.UpdateRange(movieToChangeList);
                            _context.ShoppingCartItems.UpdateRange(cartWithQuotaToAdd);

                            await _context.SaveChangesAsync();
                        }
                        else if (movieToChangeList != null && movieToChangeList.Count != 0
                          && cartWithQuotaToAdd.Count == 0 && cartWithQuotaToRemove.Count != 0)
                        {
                            _context.Movies.UpdateRange(movieToChangeList);
                            /*_context.ShoppingCartItems.RemoveRange(cartWithQuotaToRemove);*/

                            await _context.SaveChangesAsync();
                        }
                    }

                    // Movie Deletion Non Price Quota.
                    var movieToChangeNonQuota = new Movie();
                    var movieToChangeListNonQuota = new List<Movie>();

                    var carItemsWithoutQuota = _context.ShoppingCartItems
                       .Where(w => w.MyCartUserId == user.Id)
                       .Where(w => w.IsMinimumQuota == false)
                       .Where(w => dto.ShoppingCartId.Contains(w.Id))
                       .ToList();

                    var cartWithoutQuotaToRemove = new List<ShoppingCartItem>();
                    var cartWithoutQuotaToAdd = new List<ShoppingCartItem>();

                    if (myFinalCartWithoutQuota.Count != 0)
                    {
                        foreach (var item in myFinalCartWithoutQuota)
                        {
                            // decrease the amount of stock and adding the sold count in movie
                            movieToChangeNonQuota = movieSelectedList.FirstOrDefault(w => w.Id == item.MovieId);

                            if (movieToChangeNonQuota != null)
                            {
                                if (movieToChangeNonQuota.StockCount > 0)
                                {
                                    if (movieToChangeNonQuota.StockCount - item.Amount < MINIMUM_STOCK_COUNT)
                                    {
                                        var excesssProductNumber = Math.Abs(movieToChangeNonQuota.StockCount - item.Amount);
                                        var exactAmount = item.Amount - excesssProductNumber;

                                        var StockCountResult = movieToChangeNonQuota.StockCount - exactAmount;

                                        _placeOrder.AddingMovieToChangeNonQuotav2(movieToChangeNonQuota,
                                            movieToChangeListNonQuota, exactAmount, StockCountResult);
 
                                        // Left some available
                                        item.Amount -= exactAmount;
                                        cartWithoutQuotaToAdd.Add(item);
                                    }
                                    else if (movieToChangeNonQuota.StockCount - item.Amount >= MINIMUM_STOCK_COUNT)
                                    {
                                        /* movieToChangeNonQuota.Sold += item.Amount;
                                         movieToChangeNonQuota.StockCount -= item.Amount;

                                         movieToChangeListNonQuota.Add(movieToChangeNonQuota);*/

                                        _placeOrder.AddingMovieToChangeNonQuota(movieToChangeNonQuota,
                                            movieToChangeListNonQuota, item.Amount, item.Amount);

                                        // Remove all Existing used cart.
                                        cartWithoutQuotaToRemove.Add(item);

                                    }
                                }
                                else if (movieToChangeNonQuota.StockCount == 0)
                                {

                                    var shoppingCart = _context.ShoppingCartItems
                                        .Where(w => w.MyCartUserId == user.Id)
                                        .Where(w => dto.ShoppingCartId.Contains(w.Id))
                                        .ToList();

                                    bool userExistsInMovieStock = await _placeOrder.UserExistsInMovieOutOfStock(user, dto);

                                    if (userExistsInMovieStock == false)
                                    {
                                        await _placeOrder.UserLoginMovieOutOfStockNotifyList(user, dto, now);
                                        // todoo
                                        return BadRequest("The stocks of selected product is now out of stock. Wait for the re-stocking. v1");
                                    }
                                    else if (userExistsInMovieStock == true)
                                        return BadRequest("The stocks of selected product is now out of stock. Wait for the re-stocking. v2");
                                }
                            }
                        }

                        if (movieToChangeListNonQuota != null && movieToChangeListNonQuota.Count != 0
                            && cartWithoutQuotaToAdd.Count != 0 && cartWithoutQuotaToRemove.Count == 0)
                        {
                            _context.Movies.UpdateRange(movieToChangeListNonQuota);
                            _context.ShoppingCartItems.UpdateRange(cartWithoutQuotaToAdd);

                            await _context.SaveChangesAsync();
                        }
                        else if (movieToChangeListNonQuota != null && movieToChangeListNonQuota.Count != 0
                            && cartWithoutQuotaToAdd.Count == 0 && cartWithoutQuotaToRemove.Count != 0)
                        {
                            _context.Movies.UpdateRange(movieToChangeListNonQuota);
                            // It also removed the ShoppingVoucher because of the Cascade delete.
                            /*_context.ShoppingCartItems.RemoveRange(cartWithoutQuotaToRemove);*/

                            await _context.SaveChangesAsync();
                        }
                    }

                    await _placeOrder.DecreasingVoucherQuantity(user.Id, now);
                    // Decreased the Discounted Shop Quantity accordingly.
                    await _placeOrder.DecreasingShopQuantity(dto, user.Id, now);
 
                    // Goal: If the item.Amount > stockcount and was sucecssfully an place order
                    // it nees to have a notification to that user and the remaining of his cart would be remove.
 
                    return Ok("Successfully placed an order");
                }
            }
            return BadRequest("Ooops. Place Order doesnt exists");
        }

        private  double ReachedPriceQuota(Movie movieSelected, ShoppingCartItem item, int validAmount)
        {
            return item.Amount > movieSelected.StockCount ? validAmount * item.Price
                                                : item.Amount <= movieSelected.StockCount ? item.Amount * item.Price : 0;
        }

        private string GenerateHeader(string name)
        {
            return $"You have successfully purchased the {name}";
        }
        private string GenerateMessageBody(string userName1, string userName2)
        {
            return $"Hi {userName1} your purchased product is " +
                $"now prepared by {userName2}";
        }
  
        private void ClearMyFinalCartItems(List<ShoppingCartItem> myFinalCart, string id)
        {
            
            /*var obj = _context.ShoppingCartItems
                .Where(w => w.IsSelected == true)
                .Where(w=> w.MyCartUserId == id)
                .ToList();*/
            _context.RemoveRange(myFinalCart);
             
        }

        // Needs to have a view that shows if the product was successfully delivered.
        [HttpGet("VendorProductsHadBeenPurchased")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<List<VendorBoughProductsView>>> VendorProductsHadBeenPurchased()
        {
            var user = await _userManager.GetUserAsync(User);
            var transaction = await (from t in _context.ProductTransactions
                                     where t.VendorId == user.Id
                                     select new VendorBoughProductsView()
                                     {
                                         TotalAmount = t.PlaceOrderItems.Price * t.PlaceOrderItems.Amount,
                                         MovieId = t.PlaceOrderItems.MovieId,
                                         PlaceOrderId = t.PlaceOrderItemsId,
                                         ConsumerName = t.PlaceOrderItems.UserPlaceOrder.UserName,
                                         ProductTransactionId = t.Id,
                                         RiderName = t.Rider.UserName,
                                         SuccessfullyDelivered = t.VendorApproved == true,
                                         MovieName = t.PlaceOrderItems.Movie.Name,
                                         MoviePicture = t.PlaceOrderItems.Movie.ImageURL,
                                         MoviePrice = t.PlaceOrderItems.Price,
                                         PlaceOrderShippingFee=t.PlaceOrderItems.ShippingFee,
                                         OrderTime = t.PlaceOrderItems.PlacedOrderCreation.ToString("dd/M/yyyy HH:mm tt", CultureInfo.InvariantCulture)
                                     }).AsNoTracking().ToListAsync();
            return transaction;
        }
        [HttpGet("VendorPickupDetails")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<List<PickupDetailsView>>> VendorPickupDetails()
        {
            var user = await _userManager.GetUserAsync(User);
            var transaction = await (from t in _context.ProductTransactions
                                     where t.VendorId == user.Id
                                     select new PickupDetailsView()
                                     {
                                         CustomerAddress = t.PlaceOrderItems.UserPlaceOrder.Address,
                                         CustomerName = t.PlaceOrderItems.UserPlaceOrder.UserName,
                                         CustomerContactNumber = t.PlaceOrderItems.UserPlaceOrder.PhoneNumber,
                                         PlaceOrderId = t.PlaceOrderItemsId,
                                         PickUpDate = t.PickupDate.ToString("dd/M/yyyy", CultureInfo.InvariantCulture)
                                     }).AsNoTracking().ToListAsync();
            return transaction;
        }

        [HttpPut("SellerProductMarkingForDeliver")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult> BoughProductsToDeliver(SettingPickupDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var productTransactionExist = await _context.ProductTransactions.AnyAsync(w => w.Id == dto.ProductTransactionId);
            if (productTransactionExist == false) return BadRequest("Product transaction Id not found");
            var productTransaction = await _context.ProductTransactions
                .Include(w=> w.PlaceOrderItems).ThenInclude(w=> w.Movie)
                .Include(w=> w.Consumer)
                .Where(w => w.VendorId == user.Id)
                .FirstOrDefaultAsync(w => w.Id == dto.ProductTransactionId);
            var response = new ProductTransactionResponse();
            var now = DateTime.Now;
            string VENDOR_SHIP_THE_PARCEL = "Vendor had prepared the parcel and is now ready to ship";
            if (productTransaction != null && productTransaction.IsOkayForDeliver == false)
            {
                response.ProductTransactionId = productTransaction.Id;
                response.Message = VENDOR_SHIP_THE_PARCEL;
                response.ResponseCreation = DateTime.Now;

                await _context.ProductTransactionResponses.AddAsync(response);

                // need to have specific date to pickup the rider
                // 3 days maximum from the customer product date ordered
                if(dto.PickupTime > now && dto.PickupTime <= now.AddDays(3))
                {
                    productTransaction.PickupDate = dto.PickupTime;

                }
                else
                {
                    return BadRequest("3 days maximum from the customer product date ordered");
                }
                productTransaction.IsOkayForDeliver = true;
                _context.Update(productTransaction);
                await _context.SaveChangesAsync();

                var notif = new ProductTransactionNotif();

                notif.Header = "Vendor had prepared the parcel and is now ready to ship";
                notif.MessageBody = $"Hi {productTransaction.Consumer.UserName}, your product is now ready to deliver.";
                notif.Creation = DateTime.Now;
                notif.ProductImage = productTransaction.PlaceOrderItems.Movie.ImageURL;
                notif.ProductTransactionId = productTransaction.Id;
                notif.ReceivingUserId = productTransaction.ConsumerId;

                await _context.ProductTransactionNotif.AddAsync(notif);
                await _context.SaveChangesAsync();

                return Ok($"The product reference Id #{productTransaction.PlaceOrderItemsId} is now open for delivery riders. Just wait to pick them up within this hour.");
            }
            else
            {
                return BadRequest($"The place order #{productTransaction?.Id} is already open for delivery.");
            }


            return BadRequest("Something went wrong");
        }
        [HttpGet("GetAvailableDeliver")]
        [Authorize(Roles = "Rider")]
        public async Task<ActionResult<List<GetAvailableDeliverView>>> GetAvailableDeliver()
        {
            var user = await _userManager.GetUserAsync(User);
            var toDeliver = await (from t in _context.ProductTransactions
                                   where t.Rider == null
                                   where t.IsOkayForDeliver == true
                                   select new GetAvailableDeliverView()
                                   {
                                       ConsumerName = t.PlaceOrderItems.UserPlaceOrder.FirstName + " " + t.PlaceOrderItems.UserPlaceOrder.LastName,
                                       VendorName = t.PlaceOrderItems.Vendor.FirstName + " " + t.PlaceOrderItems.Vendor.LastName,
                                       LocationToDeliverParcel = t.PlaceOrderItems.UserPlaceOrder.Address,
                                       LocationToPickupParcel = t.PlaceOrderItems.Vendor.Address,
                                       ProductTransactionId = t.Id
                                   }).AsNoTracking().ToListAsync();
            return toDeliver;
        }
        // Rider pick up the deliverParcel must notify the vendor and consumer user respectively.
        [HttpPut("RiderAcceptingDelivery/{productTransactionId}")]
        [Authorize(Roles = "Rider")]
        public async Task<ActionResult> RiderAcceptingDelivery(int productTransactionId)
        {
            var user = await _userManager.GetUserAsync(User);
            var response = new ProductTransactionResponse();
            var productTransactionExist = await _context.ProductTransactions.AnyAsync(w => w.Id == productTransactionId);
            if (productTransactionExist == false) return BadRequest("Product transaction Id not found");
            var checkIfExists = await _context.ProductTransactions.AnyAsync(w => w.Id == productTransactionId);
            if (checkIfExists)
            {
                var transaction = await _context.ProductTransactions
                    .Include(w=> w.Consumer)
                    .Include(w=> w.PlaceOrderItems).ThenInclude(w=> w.Movie)
                    .Include(w=> w.Rider)
                    .Where(w => w.RiderId == null)
                    .Where(w => w.IsOkayForDeliver == true)
                    .FirstOrDefaultAsync(w => w.Id == productTransactionId);
                if (transaction != null && transaction.RiderId == null)
                {
                    // Once the rider pick up the parcel from the vendor. It needs to have
                    // a message that notify the consumer and the vendor.
                    transaction.RiderId = user.Id;
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();

                    string VENDOR_SHIP_THE_PARCEL = $"Parcel is now is out for delivery and was carried by rider #{transaction.Rider.Id} reference number";

                    response.ProductTransactionId = transaction.Id;
                    response.ResponseCreation = DateTime.Now;
                    response.Message = VENDOR_SHIP_THE_PARCEL;

                    await _context.ProductTransactionResponses.AddAsync(response);

                    var notif = new ProductTransactionNotif();

                    notif.Header = $"Parcel is now is out for delivery and was carried by rider #{transaction.Rider.Id} reference number";
                    notif.MessageBody = $"Hi {transaction.Consumer.UserName}, your product is now out for deliver.";
                    notif.Creation = DateTime.Now;
                    notif.ProductImage = transaction.PlaceOrderItems.Movie.ImageURL;
                    notif.ProductTransactionId = transaction.Id;
                    notif.ReceivingUserId = transaction.ConsumerId;

                    await _context.ProductTransactionNotif.AddAsync(notif);
                    await _context.SaveChangesAsync();
                 

                    return Ok($"Product reference number #{transaction.Id} is in your hands. Deliver it in the {transaction.PickupDate.ToString("dddd, dd MMMM yyyy")} exact time as possible.");
                }
                else
                {
                    return BadRequest("Product Order Items already had a rider. Look for more");
                }
            }

            return BadRequest("Error occured.");
        }
        // Rider list of his delivery parcel.
        [HttpGet("MyDeliverList")]
        [Authorize(Roles = "Rider")]
        public async Task<ActionResult<List<GetAvailableDeliverView>>> MyDeliverList()
        {
            var user = await _userManager.GetUserAsync(User);
            var myDeliverList = await (from t in _context.ProductTransactions
                                       where t.RiderId == user.Id
                                       where t.IsOkayForDeliver == true
                                       select new GetAvailableDeliverView()
                                       {
                                           ConsumerName = t.PlaceOrderItems.UserPlaceOrder.FirstName + " " + t.PlaceOrderItems.UserPlaceOrder.LastName,
                                           VendorName = t.PlaceOrderItems.Vendor.FirstName + " " + t.PlaceOrderItems.Vendor.LastName,
                                           LocationToDeliverParcel = t.PlaceOrderItems.UserPlaceOrder.Address,
                                           LocationToPickupParcel = t.PlaceOrderItems.Vendor.Address,
                                           ProductTransactionId = t.Id,
                                           IsDelivered = t.OrderReceived == true
                                       }).AsNoTracking().ToListAsync();
            return myDeliverList;
        }
        // Rider delivered the product to the consumer should notify the vendor and consumer user respectively.
        [HttpPut("RiderDeliveredToConsumer/{productTransactionId}")]
        [Authorize(Roles = "Rider")]
        public async Task<ActionResult> RiderDeliveredToConsumer(int productTransactionId)
        {
            var user = await _userManager.GetUserAsync(User);
            var response = new ProductTransactionResponse();
            var checkIfExists = await _context.ProductTransactions.AnyAsync(w => w.Id == productTransactionId);
            if (checkIfExists)
            {
                var transaction = await _context.ProductTransactions
                    .Include(w=> w.Consumer)
                    .Include(w => w.PlaceOrderItems).ThenInclude(w => w.UserPlaceOrder)
                    .Include(w => w.PlaceOrderItems).ThenInclude(w => w.Movie)
                    .Include(w => w.PlaceOrderItems).ThenInclude(w => w.UserPlaceOrder)
                    .Where(w => w.RiderId == user.Id)
                    .Where(w => w.IsOkayForDeliver == true)
                    .FirstOrDefaultAsync(w => w.Id == productTransactionId);
                if (transaction != null)
                {
                    // Once the rider delivered the parcel to the consumer. It needs to have
                    // a message that notify the consumer and the vendor.

                    string VENDOR_SHIP_THE_PARCEL = $"Parcel has been delivered. Recipient: {transaction.PlaceOrderItems.UserPlaceOrder.UserName}";
                    response.ProductTransactionId = transaction.Id;
                    response.ResponseCreation = DateTime.Now;
                    response.Message = VENDOR_SHIP_THE_PARCEL;

                    await _context.ProductTransactionResponses.AddAsync(response);

                    transaction.OrderReceived = true;
                    _context.Update(transaction);



                    await _context.SaveChangesAsync();
                    return Ok($"{transaction.PlaceOrderItems.UserPlaceOrder.UserName} is now received the parcel" +
                        $" {transaction.PlaceOrderItems.Movie.Name} #{transaction.Id} order id");
                }
            }

            return BadRequest("Delivered error.");

        }

        [HttpPut("VendorProductReceivingConfirmation/{productTransactionId}")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult> VendorProductReceivingConfirmation(int productTransactionId)
        {
            var user = await _userManager.GetUserAsync(User);

            var checkIfExists = await _context.ProductTransactions.AnyAsync(w => w.Id == productTransactionId);

            var transaction = await _context.ProductTransactions
                .Include(w=> w.Consumer)
                .Include(w=> w.PlaceOrderItems).ThenInclude(w=> w.Movie)
                .Where(w=> w.VendorId == user.Id)
                .Where(w=> w.OrderPlaced == true)
                .FirstOrDefaultAsync(w => w.Id == productTransactionId);

            var now = DateTime.Now;

            if (checkIfExists && transaction.VendorApproved == false)
            {
                transaction.VendorApproved = true;
                transaction.DateReceived = now;

                _context.ProductTransactions.Update(transaction);

                var notif = new ProductTransactionNotif();
                var notifList = new List<ProductTransactionNotif>();
                // Customer notification
                notif = new ProductTransactionNotif
                {
                    Header = $"Congratulation! Your parcel is now delivered",
                    MessageBody = $"Hi {transaction.Consumer.UserName}, Mark it now as Order received",
                    Creation = now,
                    ProductImage = transaction.PlaceOrderItems.Movie.ImageURL,
                    ProductTransactionId = transaction.Id,
                    ReceivingUserId = transaction.ConsumerId
                };
                notifList.Add(notif);

/*                notif.Header = $"Congratulation! Your parcel is now delivered";
                notif.MessageBody = $"Hi {transaction.Consumer.UserName}, Mark it now as Order received";
                notif.Creation = now;
                notif.ProductImage = transaction.PlaceOrderItems.Movie.ImageURL;
                notif.ProductTransactionId = transaction.Id;
                notif.ReceivingUserId = transaction.ConsumerId;*/
    

                notif = new ProductTransactionNotif
                {
                    Header = $"{transaction.Consumer.UserName} has bought and is now your certified customer.",
                    MessageBody = $"Hi {user.UserName}, {transaction.Consumer.UserName} " +
                    $"bought a {transaction.PlaceOrderItems.Amount}(x) amounting ${transaction.PlaceOrderItems.Price} {transaction.PlaceOrderItems.Movie.Name}.",
                    Creation = now,
                    ProductImage = transaction.PlaceOrderItems.Movie.ImageURL,
                    ProductTransactionId = transaction.Id,
                    ReceivingUserId = user.Id
                };

/*                notif.Header = $"{transaction.Consumer.UserName} has bought and is now your certified customer.";
                notif.MessageBody = $"Hi {user.UserName}, {transaction.Consumer.UserName} " +
                    $"bought a {transaction.PlaceOrderItems.Amount}(x) amounting ${transaction.PlaceOrderItems.Price} {transaction.PlaceOrderItems.Movie.Name}.";
                notif.Creation = now;
                notif.ProductImage = transaction.PlaceOrderItems.Movie.ImageURL;
                notif.ProductTransactionId = transaction.Id;
                notif.ReceivingUserId = user.Id;*/

                notifList.Add(notif);

                await _context.ProductTransactionNotif.AddRangeAsync(notifList);
                await _context.SaveChangesAsync();
                return Ok($"You now set the #{transaction.Id} transaction Id as delivered to the customer");

            }

            return BadRequest("Confirmation Error");
        }
        private async Task CouponMovieRefresher(List<Voucher> checkCouponMovie, int couponId,
            int couponDiscount, List<ShoppingCartItem> shoppingCouponCart)
        {
            if (checkCouponMovie != null && checkCouponMovie.Count > 0)
            {
                couponId = checkCouponMovie.FirstOrDefault().Id;

                var shopeeCouponVoucher = _context.ShoppingVouchers
                .Where(w => w.VoucherId == couponId)
                .ToList();

                couponDiscount = checkCouponMovie.FirstOrDefault().DiscountPercentage;

                _context.AttachRange(shoppingCouponCart);

                shoppingCouponCart.ForEach(w =>
                {
                    w.IsCoupon = false;
                    w.TotalDiscount -= couponDiscount;
                });
                _context.ShoppingVouchers.RemoveRange(shopeeCouponVoucher);
                await _context.SaveChangesAsync();
                /*return BadRequest("Coupon is expires, Your Cart has been refresh. Check the details before you order.");*/
            }
        }
        private async Task VoucherMovieRefresher(List<Voucher> checkVoucherMovie, int voucherxId,
           int voucherDiscount, List<ShoppingCartItem> shoppingVoucherCart)
        {
            if (checkVoucherMovie != null || checkVoucherMovie.Count != 0)
            {
                voucherxId = checkVoucherMovie.FirstOrDefault().Id;

                var shopeeVoucheroucher = _context.ShoppingVouchers
                    .Where(w => w.VoucherId == voucherxId)
                    .ToList();

                voucherDiscount = checkVoucherMovie.FirstOrDefault().DiscountPercentage;

                _context.AttachRange(shoppingVoucherCart);

                shoppingVoucherCart.ForEach(w =>
                {
                    w.IsVoucher = false;
                    w.TotalDiscount -= voucherDiscount;
                });

                _context.ShoppingVouchers.RemoveRange(shopeeVoucheroucher);

                await _context.SaveChangesAsync();
            }

        }

        // Consumer to see his purchase order history
        [HttpGet("user/purchase/order/{placeOrderItemId}")]
        public async Task<ActionResult<UserPurchasedOrderedInfoView>> 
            UserPurchasedOrderedInfo(int placeOrderItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            // aso

            var orders = await _context.PlaceOrderItems
                .Where(w=> w.UserPlaceOrderId == user.Id)
                .FirstOrDefaultAsync(w => w.Id == placeOrderItemId);

            var orderProgress = await (from s in _context.PlaceOrderItems
                                       join sc in _context.ProductTransactions
                                       on s.Id equals sc.PlaceOrderItemsId
                                       where s.UserPlaceOrderId == user.Id
                                       where s.Id == placeOrderItemId
                                       select new OrderProgression
                                       {
                                           OrderId = s.Id,
                                           OrderPlaced = sc.OrderPlaced,
                                           OrderReceived = sc.OrderReceived,
                                           PaymentConfirmed = sc.PaymentConfirmed,
                                           ToRate = sc.ToRate,
                                           DeliveryAddress = s.UserPlaceOrder.Address,
                                           PhoneNumber = s.UserPlaceOrder.PhoneNumber,
                                           TransactionResponses = (from f in sc.ProductTransactionResponse
                                                                  /* where f.ProductTransaction.PlaceOrderItemsId == placeOrderItemId*/
                                                                   select new TransactionResponse()
                                                                   {
                                                                       DateCreation = f.ResponseCreation,
                                                                       Message=f.Message,
                                                                       ResponseId=f.Id
                                                                   }).OrderBy(w=> w.DateCreation).ToList()
                                       }).AsNoTracking().FirstOrDefaultAsync();

            var purchasedOrder = await (from p in _context.PlaceOrderItems
                                        where p.Id ==placeOrderItemId
                                        select new PurchasedOrder
                                        {
                                              Amount = p.Amount,
                                              Total = p.Amount * p.Price,
                                              DatePurchased = p.PlacedOrderCreation,
                                              MovieId=p.MovieId,
                                              MovieName=p.Movie.Name,
                                              ImageUrl=p.Movie.ImageURL,
                                        }).AsNoTracking().FirstOrDefaultAsync();

            return new UserPurchasedOrderedInfoView()
            {
                OrderProgressionView = orderProgress,
                PurchasedOrders = purchasedOrder
            };
        }
        [HttpPost("RateProduct")]
        public async Task<ActionResult> RateProduct([FromForm] RatePurchasedOrder dto)
        {
            var user = await _userManager.GetUserAsync(User);

            // Check if you had bought an specific product
            var myOrder = await _context.PlaceOrderItems
                .Where(w => w.UserPlaceOrderId == user.Id)
                .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);
            if (myOrder == null) return NotFound("You dont have any place order in that movie id");
            /*var movie = await _context.Movies.FirstOrDefaultAsync(w => w.Id == dto.MovieId);*/

            bool checkMovieExists = await _context.Movies.AnyAsync(w => w.Id == dto.MovieId);
            // consumer already had a record to the product transaction which means the consumer
            // has a successful order to the product.
            var didIReceived = await _context.ProductTransactions
                .Where(w => w.VendorId == myOrder.VendorId)
                .Where(w => w.OrderReceived == true)
                .FirstOrDefaultAsync(w => w.ConsumerId == user.Id);

            var userRating = await _context.ProductTransactions
                .Include(w=> w.PlaceOrderItems)
                .Where(w => w.VendorId == myOrder.VendorId)
                .Where(w => w.PlaceOrderItems.MovieId == myOrder.MovieId)
                .Where(w => w.OrderReceived == true)
            
                .FirstOrDefaultAsync(w => w.ConsumerId == user.Id);

            var didIPurchase = await _context.ProductTransactions
                .Where(w=> w.ConsumerId==user.Id&& w.ToRate==true)
                .AnyAsync();

            var ratingBoughtProduct = new RateProduct();

            if (checkMovieExists)
            {
                var myRate = await _context.RateProducts
                   .Where(w => w.UserWhoHadRateId == user.Id)
                   .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

                var now = DateTime.Now;
                // User could changed their rated product to the vendor
                // Only in 15 days from rhe consumer received the product.
                // If the consumer could only changed the rate product only once.

                // If the rateEndedDate is less than now. Frontend need to prevent to reflect the button.
                if (myRate != null && myRate.RateEndedDate >= now && myRate.IsEnableToComment == false)
                {
                    // If I rated the product before.
                    myRate.RateCount = dto.Rate;
                    myRate.Message = dto.Message;
                    myRate.RateCreation = now;
                    myRate.IsEnableToComment = true;
                    _context.RateProducts.Update(myRate);
                    await _context.SaveChangesAsync();

                    // It will update the rate along with modification of myRate.RateCount
                    userRating.Rate = dto.Rate;
                    userRating.RateId = myRate.Id;

                    _context.ProductTransactions.Update(userRating);

                    await _context.SaveChangesAsync();
                    return Ok($"Successfully changed star rate into {myRate.RateCount}");
                }
                else if (myRate == null && userRating.OrderReceived == true)
                {
                    // The user could rate the problem if the Order received is now true.
                    ratingBoughtProduct.MovieId = myOrder.MovieId;
                    ratingBoughtProduct.RateCreation = now;
                    ratingBoughtProduct.RateEndedDate = now.AddDays(15);
                    ratingBoughtProduct.RateCount = dto.Rate;
                    ratingBoughtProduct.UserWhoHadRateId = user.Id;
                    ratingBoughtProduct.Message = dto.Message;

                    if (dto.ImageUrl != null)
                        ratingBoughtProduct.ImageUrl = await fileStorageService.SaveFile(container, dto.ImageUrl);

                    await _context.RateProducts.AddAsync(ratingBoughtProduct);
                    await _context.SaveChangesAsync();

                    // The consumer has reached the last step which is to rate the product.
                    userRating.ToRate = true;
                    userRating.Rate = dto.Rate;
                    userRating.RateId = ratingBoughtProduct.Id;

                    _context.ProductTransactions.Update(userRating);
                    await _context.SaveChangesAsync();

                    return Ok($"Successfully rated a " +
                        $"{ratingBoughtProduct.RateCount} star.");
                }
                else
                {
                    return BadRequest("Sorry you already used your remaining product rating reaction");
                }
            }
            return BadRequest("Something went wrong");
        }
        
    }
}
