using Application.BaseRepository;
using Application.Repository.IService;
using Application.Repository.Service.ShoppingProcessor.Interface;
using Application.ViewModel.ShoppingCart;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service
{
    public class ShoppingCartRepository :  IShoppingCart
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IShoppingDiscountHall _shoppingDiscountHall;
        private readonly IBaseRepository<ShoppingCartItem> _base;

        public ShoppingCartRepository(UserManager<ApplicationUser> userManager,
            ApplicationDbContext context, IShoppingDiscountHall shoppingDiscountHall, IBaseRepository<ShoppingCartItem> @base)
        {
            _userManager = userManager;
            _context = context;
            _shoppingDiscountHall = shoppingDiscountHall;
            _base = @base;
        }

        public async Task RefreshCart(string userId)
        {

            var now = DateTime.Now;

            var allCart = await _context.ShoppingCartItems
                .Where(w => w.MyCartUserId == userId)
                .Include(w => w.Movie)
                .ToListAsync();

            // Shopping Cart with expired voucher
            var shoppingCartVoucher = await ExpiredVoucheres(now);

            var cartToAddList = new List<ShoppingCartItem>();
            var voucherNotAvailable = _context.Vouchers
                            .Where(w => w.Quantity == 0 || w.Expire <= now).ToList();

            var myVoucher = new Voucher();
            if (shoppingCartVoucher != null)
            {
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
 
                    var shopVoucher = _context.ShoppingVouchers.FirstOrDefault(w => w.Id == item.Id);
                    if (shopVoucher != null)
                        _context.ShoppingVouchers.Remove(shopVoucher);

                }
            }
            if(cartToAddList != null || cartToAddList.Count != 0)
            {
                _context.ShoppingCartItems.UpdateRange(cartToAddList);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RefreshCart(string userId, List<int?> shoppingCartId)
        {
            var now = DateTime.Now;

            var allCart = await _context.ShoppingCartItems
                .Where(w=> shoppingCartId.Contains(w.Id))
                .Where(w => w.MyCartUserId == userId)
                .Include(w => w.Movie)
                .ToListAsync(); 

            // Shopping Cart with expired voucher
            var shoppingCartVoucher = await ExpiredVoucheres(now);

            var cartToAddList = new List<ShoppingCartItem>();

            var myVoucherNotAvailable = _context.Vouchers
                            .Where(w=> allCart.Select(w=> w.MovieId).Contains(w.MovieId))
                            .Where(w => w.Quantity == 0 || w.Expire <= now)
                            .ToList();

            var myVoucher = new Voucher();
            var myVoucherExpireToAdd = new List<Voucher>();
            var removingMyShoppingVoucher = new List<ShoppingVoucher>();

            if (shoppingCartVoucher != null)
            {
                /*foreach (var item in shoppingCartVoucher)
                {

                    var cart = allCart.FirstOrDefault(w => w.Id == item.ShoppingCartItemId);

                    myVoucher = myVoucherNotAvailable.FirstOrDefault(w => w.Id == item.VoucherId);
                     
                    if (cart != null)
                    {
                        var cartDiscount = item.Voucher.DiscountPercentage;
                        var cartProductPrice = cart.Movie.Price;

                        var discountedPrice = (decimal)cartProductPrice * (cartDiscount / 100m);

                        cart.Price += (double)discountedPrice;

                        if(cart.MovieId == myVoucher.MovieId && cart.IsCoupon == true && cart.IsVoucher == false
                            && myVoucher.Code != null )
                        {
                            // Coupon
                            cart.IsCoupon = false;
                            cart.TotalDiscount -= myVoucher.DiscountPercentage;
                        }
                        else if (cart.MovieId == myVoucher.MovieId && cart.IsVoucher == true && cart.IsCoupon == false
                            && myVoucher.Code == null)
                        {
                            // Voucher
                            cart.IsVoucher = false;
                            cart.TotalDiscount -= myVoucher.DiscountPercentage;

                        }

                        cartToAddList.Add(cart);
                        myVoucherExpireToAdd.Add(myVoucher);
                        removingMyShoppingVoucher.Add(item);
                    }

                }*/
                Samp(allCart, shoppingCartVoucher, cartToAddList, myVoucherNotAvailable,
                    myVoucher, myVoucherExpireToAdd, removingMyShoppingVoucher);
            }
            if ((cartToAddList != null || cartToAddList.Count != 0) && removingMyShoppingVoucher.Count != 0)
            {
                _context.ShoppingCartItems.UpdateRange(cartToAddList);
                /*_context.Vouchers.RemoveRange(myVoucherExpireToAdd);*/
                _context.ShoppingVouchers.RemoveRange(removingMyShoppingVoucher);
                await _context.SaveChangesAsync();
            }
        }

        private static void Samp(List<ShoppingCartItem> allCart, List<ShoppingVoucher> shoppingCartVoucher, List<ShoppingCartItem> cartToAddList, List<Voucher> myVoucherNotAvailable, Voucher myVoucher,
            List<Voucher> myVoucherExpireToAdd, List<ShoppingVoucher> removingMyShoppingVoucher)
        {
            foreach (var item in shoppingCartVoucher)
            {
                var cart = allCart.FirstOrDefault(w => w.Id == item.ShoppingCartItemId);

                myVoucher = myVoucherNotAvailable.FirstOrDefault(w => w.Id == item.VoucherId);

                if (cart != null)
                {
                    var cartDiscount = item.Voucher.DiscountPercentage;
                    var cartProductPrice = cart.Movie.Price;

                    var discountedPrice = (decimal)cartProductPrice * (cartDiscount / 100m);

                    cart.Price += (double)discountedPrice;
 
                    if (VoucherOrCouponDiscount(myVoucher, cart, cart.IsCoupon, cart.IsVoucher, true))
                    {
                        // Coupon
                        cart.IsCoupon = false;
                        cart.TotalDiscount -= myVoucher.DiscountPercentage;
                    }
                    else if (VoucherOrCouponDiscount(myVoucher, cart, cart.IsVoucher, cart.IsCoupon, false))
                    {
                        // Voucher
                        cart.IsVoucher = false;
                        cart.TotalDiscount -= myVoucher.DiscountPercentage;
                    }

                    cartToAddList.Add(cart);
                    myVoucherExpireToAdd.Add(myVoucher);
                    removingMyShoppingVoucher.Add(item);
                }

            }
 
        }
        private static bool VoucherOrCouponDiscount(Voucher myVoucher, ShoppingCartItem cart,bool typeVoucherTrue, bool typeVoucherFalse,
            bool voucherCodeNullOrNot)

        {
            bool voucherCodeCondition = voucherCodeNullOrNot == true ? myVoucher.Code != null : myVoucher.Code == null;

            return cart.MovieId == myVoucher.MovieId && typeVoucherTrue == true && typeVoucherFalse == false /*cart.IsVoucher == false*/
                                    && voucherCodeCondition /*myVoucher.Code != null*/;
        }
        private async Task<List<ShoppingVoucher>> ExpiredVoucheres(DateTime now)
        {
            return await _context.ShoppingVouchers
                            .Include(w => w.Voucher)
                            .Include(w => w.ShoppingCartItem)
                            .Where(w => w.Voucher.Expire <= now || w.Voucher.Quantity == 0)
                            .ToListAsync();
        }

        public async Task PutChangeDiscount(string userId, AddItemToCartManyDTO dto)
        {
            var movie = await _context.Movies
                            .FirstOrDefaultAsync(w => w.Id == dto.MovieId);

            var now = DateTime.Now;

            var vendorShopQuotaMovie = await _context.DiscountedShop
                    .Where(w => w.Quantity > 0 && w.Expire >= now)
                    .Where(w => w.VendorId == movie.VendorId)
                    .ToListAsync();

            var cart = new ShoppingCartItem();

            // Error discount changing in the view // Fix
            var voucherWithoutCouponCart = await _context.ShoppingCartItems
                              .Where(w => w.MyCartUserId == userId)
                              .Where(w => w.IsVoucher == true && w.IsCoupon == false)
                              .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            var couponWithVoucherCart = await _context.ShoppingCartItems
                              .Where(w => w.MyCartUserId == userId)
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
                   .Where(w => w.Code != null)
                   .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            var discountedCart = new DiscountShop_Cart();
            var shoppingVoucher = new ShoppingVoucher();

            if (voucherWithoutCouponCart != null)
            {
                discountedCart = _context.DiscountShop_Cart
                   .Include(w => w.DiscountedShop)
                   .Where(w => w.DiscountedShop.VendorId == voucherWithoutCouponCart.VendorId)
                   .FirstOrDefault(w => w.ShoppingCartItemId == voucherWithoutCouponCart.Id);
            }

            // Get the Coupon to remove.
            if (couponWithVoucherCart != null)
            {
                shoppingVoucher = await _context.ShoppingVouchers
                    .Include(w => w.Voucher)
                    .Where(w => w.ShoppingVoucherUserid == userId)
                    .Where(w => w.Voucher.Code != null)
                    .FirstOrDefaultAsync(w => w.ShoppingCartItemId == couponWithVoucherCart.Id);
            }

            if (vendorShopQuotaMovie != null || vendorShopQuotaMovie.Count != 0)
            {
                if (couponExists != null && discountedCart != null)
                {
                    /*voucherWithoutCouponCart.IsCoupon = true;
                    voucherWithoutCouponCart.TotalDiscount += couponExists.DiscountPercentage;
                    voucherWithoutCouponCart.IsMinimumQuota = false;*/

                    /*PriceQuotaAndCouponAdjustment(true, true, false, voucherWithoutCouponCart, couponExists);*/
                    _shoppingDiscountHall.PriceQuotaAndCouponAdjustment(true, true, false, voucherWithoutCouponCart, couponExists);

                    /*await AddingShoppingVoucher(voucherWithoutCouponCart.Id, couponList, userId);*/
                    await _shoppingDiscountHall.AddingShoppingVoucher(voucherWithoutCouponCart.Id, couponList, userId);
                    if (discountedCart != null)
                        // remove the quota discount cart.
                        _context.DiscountShop_Cart.Remove(discountedCart);

                    await _context.SaveChangesAsync();
                    /*return Ok("Successfully edit your cart. Coupon is now applied but Price quota is dismantle");*/

                }
                else if (couponExists == null && couponWithVoucherCart != null)
                {
                    /*couponWithVoucherCart.IsCoupon = false;
                    couponWithVoucherCart.TotalDiscount -= couponValueCart.DiscountPercentage;
                    couponWithVoucherCart.IsMinimumQuota = true;*/

                    /* PriceQuotaAndCouponAdjustment(false, false, true, couponWithVoucherCart, couponValueCart);*/

                    _shoppingDiscountHall.PriceQuotaAndCouponAdjustment(false, false, true, couponWithVoucherCart, couponValueCart);

                    /*await AddDiscountedCartQuota(userId, couponWithVoucherCart, vendorShopQuotaMovie);*/

                    await _shoppingDiscountHall.AddDiscountedCartQuota(userId, couponWithVoucherCart,
                        vendorShopQuotaMovie);

                    if (shoppingVoucher != null)
                        // remove the shopee voucher
                        _context.ShoppingVouchers.Remove(shoppingVoucher);

                    await _context.SaveChangesAsync();
                    /*return Ok("Successfully edit your cart. Price quota is now applied but Coupon is off");*/
                }
            }
        }


        private static void PriceQuotaAndCouponAdjustment(bool IsCoupon,
            bool IsTotalDiscountAdd, bool IsMinimumQuota,ShoppingCartItem couponWithVoucherCart,
            Voucher couponValueCart)
        {
            if (IsCoupon == true)
            {
                couponWithVoucherCart.IsCoupon = true;
            }
            if (IsCoupon == false)
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

        private async Task AddDiscountedCartQuota(string userId, ShoppingCartItem cart, List<DiscountedShop> vendorShopQuotaMovie)
        {
            var discountedQuota = (from p in vendorShopQuotaMovie
                                   select new DiscountShop_Cart()
                                   {
                                       ShoppingCartItemId = cart.Id,
                                       DiscountedShopId = p.Id,
                                       ShoppingVoucherUserid = userId,
                                   }).ToList();

            await _context.DiscountShop_Cart.AddRangeAsync(discountedQuota);
            await _context.SaveChangesAsync();
        }
        public async Task UserLoginMovieOutOfStockNotify(int movieId, ApplicationUser user,
            DateTime now, Movie movie)
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

        public async Task<bool> MovieOutOfStockUserExistsNotif(string userId, int movieId)
        {
            return await _context.MovieOutOfStocks
                        .Where(w => w.UserAttemptToCartOutOfStockId == userId)
                        .AnyAsync(w => w.MovieId == movieId);
        }

        public Task<bool> MovieOutOfStockUserExistsNotif(string userId, List<int> movieIds)
        {
            throw new NotImplementedException();
        }

        public async Task MessageProductNotificationRemoval(string userId)
        {
            var messageTable = await _context.MessageTables
                                    .Where(w => w.MovieRefId != 0 && w.UserHeHadMessageId == userId)
                                    .ToListAsync();

            _context.MessageTables.RemoveRange(messageTable);
            await _context.SaveChangesAsync();
        }
    }
}
