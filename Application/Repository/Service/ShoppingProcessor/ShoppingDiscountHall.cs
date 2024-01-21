using Application.Repository.Service.ShoppingProcessor.Interface;
using Domain;
using Persistence;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using Application.ViewModel.ShoppingCart;
using Microsoft.EntityFrameworkCore;
using Application.Repository.Service.SampOCP;

namespace Application.Repository.Service.ShoppingProcessor
{
    public class ShoppingDiscountHall : IShoppingDiscountHall
    {
        private readonly ApplicationDbContext _context;
        /*private readonly ApplicationDbContext _context;*/
        public ShoppingDiscountHall(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddDiscountedCartQuota(string userId, ShoppingCartItem cart, List<DiscountedShop> vendorShopQuotaMovie)
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
        public async Task AddingShoppingCartNoCouponNoVoucher(ApplicationUser user, ShoppingCartItem cart, Movie movie, decimal finalPrice, int shippingFee)
        {
            cart.MovieId = movie.Id;
            cart.Amount = 1;
            cart.ShippingFee = shippingFee;
            cart.MyCartUserId = user.Id;
            cart.DateAddToCart = DateTime.Now;
            cart.IsSelected = true;
            cart.IsCoupon = false;
            cart.IsVoucher = false;
            cart.Price = (double)(finalPrice);
            cart.VendorId = movie.VendorId;

            await _context.ShoppingCartItems.AddAsync(cart);
            await _context.SaveChangesAsync();
        }
        public async Task AddingShoppingCartNoCouponNoVoucherWithQuota(ApplicationUser user, ShoppingCartItem cart, Movie movie, decimal finalPrice, int shippingFee)
        {
            cart.MovieId = movie.Id;
            cart.Amount = 1;
            cart.ShippingFee = shippingFee;
            cart.MyCartUserId = user.Id;
            cart.DateAddToCart = DateTime.Now;
            cart.IsSelected = true;
            cart.IsMinimumQuota = true;
            cart.IsCoupon = false;
            cart.IsVoucher = false;
            cart.Price = (double)(finalPrice);
            cart.VendorId = movie.VendorId;
            await _context.ShoppingCartItems.AddAsync(cart);
            await _context.SaveChangesAsync();
        }
        // ex1
        // Translate the paremeter into interface.
        public async Task<ShoppingCartItem> AddingShoppingCartWithCouponNoVoucher(string userId, DateTime now,
            Movie movie, decimal finalPrice, int movieShipping, decimal totalCouponDiscount)
        {
            var cart = new ShoppingCartItem()
            {
                MovieId = movie.Id,
                Amount = 1,
                MyCartUserId = userId,
                TotalDiscount = totalCouponDiscount,
                DateAddToCart = now,
                IsSelected = true,
                IsCoupon = true,
                IsVoucher = false,
                ShippingFee = movieShipping,
                Price = (double)finalPrice,
                VendorId = movie.VendorId
            };

            await _context.ShoppingCartItems.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }
        public ShoppingCartItem AddingSamp(ISampAddingProp sampAddingProp)
        {
            var cart = new FirstModel();

            sampAddingProp.ISampAdding.SampAddingMethod(cart);

            return new ShoppingCartItem();
        }
        public async Task AddingShoppingVoucher(int cartId, List<Voucher> voucherDiscountList, string userId)
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
        public decimal CouponTotalDiscountPrice(List<Voucher> couponList, Movie movie, ref decimal totalCouponDiscount)
        {
            decimal finalPrice;
            foreach (var item in couponList)
            {
                totalCouponDiscount += item.DiscountPercentage;
            }

            finalPrice = TotalDiscountedPrice(movie, totalCouponDiscount);
            return finalPrice;
        }
        public decimal TotalDiscountedPrice(Movie movie, decimal totalDiscount)
        {
            decimal finalPrice;
            // for couponDiscountList
            var firmItemCouponDiscountedPrice = (decimal)movie.Price * (totalDiscount / 100m);

            var couponListDiscount = (decimal)movie.Price - firmItemCouponDiscountedPrice;

            finalPrice = couponListDiscount;
            return finalPrice;
        }
        public async Task NoCouponWithVoucherAndQuotaIncreaseAmount(AddItemToCartManyDTO dto, ApplicationUser user)
        {
            var checkProductExistsNoCouponVoucherWithQuota = await _context.ShoppingCartItems
                                            .Where(w => w.MyCartUserId == user.Id)
                                            .Where(w => w.IsVoucher == false && w.IsCoupon == false)
                                            .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

            checkProductExistsNoCouponVoucherWithQuota.Amount += 1;
            _context.ShoppingCartItems.Update(checkProductExistsNoCouponVoucherWithQuota);
            await _context.SaveChangesAsync();
        }
        // Need to change the logic
        public void PriceQuotaAndCouponAdjustment(bool IsCoupon, bool IsTotalDiscountAdd, bool IsMinimumQuota, ShoppingCartItem couponWithVoucherCart, Voucher couponValueCart)
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
        }
        public DateTime DoesAllDiscountValid(DateTime now, List<Voucher> couponList)
        {
            var couponExpires = couponList.Select(w => w.Expire);
            // If the result is new DateTime() it means it doesnt expired yet;
            var doesAllValid = DoesDiscountPromotionValid(now, couponExpires);
            return doesAllValid;
        }
        public DateTime DoesDiscountPromotionValid(DateTime now, IEnumerable<DateTime> voucherExpires) => voucherExpires.FirstOrDefault(expireDate => now > expireDate);    
        public async Task AddingShoppingCartWithVoucherNoCoupon(ApplicationUser user, ShoppingCartItem cart, Movie movie, decimal finalPrice, int movieShipping, decimal totalDiscount)
        {
            cart.MovieId = movie.Id;
            cart.Amount = 1;
            cart.TotalDiscount = totalDiscount;
            cart.MyCartUserId = user.Id;
            cart.DateAddToCart = DateTime.Now;
            cart.IsSelected = true;
            cart.ShippingFee = movieShipping;
            cart.IsCoupon = false;
            cart.IsVoucher = true;
            cart.Price = (double)(finalPrice);
            cart.VendorId = movie.VendorId;
            await _context.ShoppingCartItems.AddAsync(cart);
            await _context.SaveChangesAsync();
        }
        public decimal VoucherTotalDiscountPrice(List<Voucher> voucherList, Movie movie, ref decimal totalVoucherDiscount)
        {
            decimal finalPrice;
            foreach (var item in voucherList)
            {
                totalVoucherDiscount += item.DiscountPercentage;
            }
            finalPrice = TotalDiscountedPrice(movie, totalVoucherDiscount);
            return finalPrice;
        }
        // Ex2
        public async Task<ShoppingCartItem> AddingShoppingCartWithVoucherNoCouponWithQuota(string userId, DateTime now,Movie movie,
            decimal finalPrice, int movieShipping, decimal totalDiscount)
        {
            var cart = new ShoppingCartItem();

            cart.MovieId = movie.Id;
            cart.Amount = 1;
            cart.TotalDiscount = totalDiscount;
            cart.MyCartUserId = userId;
            cart.DateAddToCart = now;
            cart.IsSelected = true;
            cart.IsMinimumQuota = true;
            cart.ShippingFee = movieShipping;
            cart.IsCoupon = false;
            cart.IsVoucher = true;
            cart.Price = (double)(finalPrice);
            cart.VendorId = movie.VendorId;

            await _context.ShoppingCartItems.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }
        public async Task AddingShoppingUniqueVoucher(string userId, ShoppingCartItem cart, ShoppingCartItem checkProductExistsNoCouponWithVoucher,
            List<Voucher> voucherList, List<ShoppingVoucher> shopeeVoucher)
        {
            var isSameCartItems = shopeeVoucher.Any(w => w.ShoppingCartItemId == checkProductExistsNoCouponWithVoucher.Id);

            if (isSameCartItems == false)
            {
                // Add a Shopping Voucher id that refence to the shoppingCartItem. 
                await AddingShoppingVoucher(cart.Id, voucherList, userId);
            }
        }
        public async Task<ShoppingCartItem> AddingShoppingCartWithCouponWithVoucher(string userId, DateTime now, Movie movie, decimal finalPrice, int movieShipping, decimal totalDiscount, bool doesHavePriceQuota)
        {
            var cart = new ShoppingCartItem()
            {
                MovieId = movie.Id,
                Amount = 1,
                MyCartUserId = userId,
                DateAddToCart = now,
                TotalDiscount = totalDiscount,
                IsSelected = true,
                IsCoupon = true,
                IsVoucher = true,
                ShippingFee = movieShipping,
                IsMinimumQuota = doesHavePriceQuota == true ? true : false,
                /*    cart.Price = (double)(finalPrice + movieShipping);*/
                Price = (double)finalPrice,
                VendorId = movie.VendorId
            };
            await _context.ShoppingCartItems.AddAsync(cart);
            await _context.SaveChangesAsync();
            return cart;
        }
        public decimal CouponAndVoucherDiscountedPrice(Movie movie, decimal finalPrice, decimal totalVoucherDiscount, decimal totalCouponDiscount)
        {
            var firmItemVoucherDiscountedPrice = (decimal)movie.Price * (totalVoucherDiscount / 100m);

            // for couponDiscountList
            var firmItemCouponDiscountedPrice = (decimal)movie.Price * (totalCouponDiscount / 100m);

            var couponListAndVoucherListDiscount = (firmItemCouponDiscountedPrice + firmItemVoucherDiscountedPrice);

            finalPrice = finalPrice - couponListAndVoucherListDiscount;
            return finalPrice;
        }
        public decimal VoucherCouponTotalDiscount(List<Voucher> voucherList, List<Voucher> couponList, ref decimal totalVoucherDiscount, ref decimal totalCouponDiscount)
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

        public int HalfPriceShippingFee(int firstHalf, int secondHalf, DateTime now, int movieShipping)
        {
            // Shipping fee 50% off

            decimal discountedPrice = movieShipping * (50 / 100m);

            int halfOfStandardPrice = movieShipping - (int)discountedPrice;

            // if every 15th and 30th the shipping fee would bee half of standard shippe fee
            // else if not 15th and 30th it would be the standard price movieShipping
            if (now.Day == firstHalf || now.Day == secondHalf)
                return halfOfStandardPrice;
            else
                return movieShipping;

        }
    }
}
