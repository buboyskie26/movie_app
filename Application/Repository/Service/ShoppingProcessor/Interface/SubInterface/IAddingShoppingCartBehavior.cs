using Application.ViewModel.ShoppingCart;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service.ShoppingProcessor.Interface.SubInterface
{
    public  interface IAddingShoppingCartBehavior : IDiscountBehavior
    {
        Task AddingShoppingVoucher(int cartId, List<Voucher> voucherDiscountList, string userId);
        Task AddDiscountedCartQuota(string userId, ShoppingCartItem cart, List<DiscountedShop> vendorShopQuotaMovie);
        Task AddingShoppingCartNoCouponNoVoucher(ApplicationUser user, ShoppingCartItem cart, Movie movie, decimal finalPrice, int shippingFee);
        Task AddingShoppingCartNoCouponNoVoucherWithQuota(ApplicationUser user, ShoppingCartItem cart, Movie movie,
            decimal finalPrice, int shippingFee);
        Task AddingShoppingCartWithVoucherNoCoupon(ApplicationUser user, ShoppingCartItem cart,
            Movie movie, decimal finalPrice, int movieShipping, decimal totalDiscount);
        Task<ShoppingCartItem> AddingShoppingCartWithVoucherNoCouponWithQuota(string userId,DateTime now,
            Movie movie, decimal finalPrice, int movieShipping, decimal totalDiscount);
        Task AddingShoppingUniqueVoucher(string userId, ShoppingCartItem cart,
            ShoppingCartItem checkProductExistsNoCouponWithVoucher, List<Voucher> voucherList, List<ShoppingVoucher> shopeeVoucher);
        Task<ShoppingCartItem> AddingShoppingCartWithCouponWithVoucher(string userId, DateTime now,
            Movie movie, decimal finalPrice, int movieShipping, decimal totalDiscount, bool doesHavePriceQuota);
        Task NoCouponWithVoucherAndQuotaIncreaseAmount(AddItemToCartManyDTO dto, ApplicationUser user);
        Task<ShoppingCartItem> AddingShoppingCartWithCouponNoVoucher(string userId, DateTime now, Movie movie,
            decimal finalPrice, int movieShipping, decimal totalCouponDiscount);
    }
}
