using Application.ViewModel.Movie;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService.SubInterface
{
    public interface IShoppingCartRefresher
    {
        
        Task CouponMovieRefresher(List<Voucher> checkCouponMovie, int couponId,
            int couponDiscount, List<ShoppingCartItem> shoppingCouponCart);
        Task VoucherMovieRefresher(List<Voucher> checkVoucherMovie, int voucherxId,
           int voucherDiscount, List<ShoppingCartItem> shoppingVoucherCart);
         

    }
    public interface IShoppingCartRefresherV2
    {
        Task DiscountMovieRefresher(List<Voucher> checkCouponMovie, int couponId,
            int couponDiscount, List<ShoppingCartItem> shoppingCouponCart, bool IsVoucherType);
    }
    public abstract class ShoppingCartRefresh : IShoppingCartRefresherV2
    {
        public abstract Task DiscountMovieRefresher(List<Voucher> checkCouponMovie, int couponId, int couponDiscount,
            List<ShoppingCartItem> shoppingCouponCart, bool IsVoucherType);

    }
}
