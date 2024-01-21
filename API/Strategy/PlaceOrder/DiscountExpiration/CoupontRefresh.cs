using Application.Repository.IService;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Strategy.PlaceOrder.DiscountExpiration
{
    public interface IDiscountRefresh
    {
        Task CartDiscountRefresh(List<Voucher> checkCouponMovie, int couponId,
            int couponDiscount, List<ShoppingCartItem> shoppingCouponCart);
        /*Task CartDiscountRefresh(bool one, bool two, bool three);*/
    }
    public class CoupontRefresh : IDiscountRefresh
    {
        private readonly IPlaceOrder _placeOrder;

        public CoupontRefresh(IPlaceOrder placeOrder)
        {
            _placeOrder = placeOrder;
        }
         
        public async Task CartDiscountRefresh(List<Voucher> checkCouponMovie, int couponId,
            int couponDiscount, List<ShoppingCartItem> shoppingCouponCart)
        {
            await _placeOrder.CouponMovieRefresher(checkCouponMovie, couponId, couponDiscount,
                shoppingCouponCart);
        }
         
    }
    public class VoucherRefresh : IDiscountRefresh
    {
        private readonly IPlaceOrder _placeOrder;

        public VoucherRefresh(IPlaceOrder placeOrder)
        {
            _placeOrder = placeOrder;
        }

        public async Task CartDiscountRefresh(List<Voucher> checkVoucherMovie, int voucherxId,
           int voucherDiscount, List<ShoppingCartItem> shoppingVoucherCart)
        {
            await _placeOrder.VoucherMovieRefresher(checkVoucherMovie, voucherxId, voucherDiscount,
                shoppingVoucherCart);
        }

    }
    public class OperationCoupon
    {
        IDiscountRefresh discountRefresh;
        public (bool, bool) CouponChoices(bool checkCouponMovie, bool checkCouponMovieCount,
            bool checkVoucherMovieCount,
            bool checkVoucherMovie, bool checkVoucherMovieCountv2,bool checkCouponMovieCountv2,
            IPlaceOrder placeOrder)
        {

            (bool First, bool Second) t2 = (false, false);
             
            if (checkCouponMovie == true && checkCouponMovieCount == true 
                && checkVoucherMovieCount == true)
            {
                discountRefresh = new CoupontRefresh(placeOrder);
                return (t2.First = true, t2.Second = false);
            }
            else if (checkVoucherMovie == true && checkVoucherMovieCountv2 == true
                && checkCouponMovieCountv2 == true)
            {
                discountRefresh = new VoucherRefresh(placeOrder);

                return (t2.First = false, t2.Second = true);

                /*return true;*/
            }

            return (t2.First, t2.Second);
        }
        public async Task TriggerOperation(List<Voucher> checkCouponMovie, int couponId,
            int couponDiscount, List<ShoppingCartItem> shoppingCouponCart)
        {
            await discountRefresh.CartDiscountRefresh(checkCouponMovie, couponId, couponDiscount,
                shoppingCouponCart);
        }
    }
    public class OperationVoucher
    {
        IDiscountRefresh discountRefresh;
        public bool VoucherChoices(bool checkVoucherMovie, bool checkVoucherMovieCount,
            bool checkCouponMovieCount, IPlaceOrder placeOrder)
        {
            if (checkVoucherMovie == true && checkVoucherMovieCount == true
                && checkCouponMovieCount == true)
            {
                discountRefresh = new VoucherRefresh(placeOrder);
                return true;
            }

            return false;
        }
        public async Task TriggerOperation(List<Voucher> checkCouponMovie, int couponId,
            int couponDiscount, List<ShoppingCartItem> shoppingCouponCart)
        {
            await discountRefresh.CartDiscountRefresh(checkCouponMovie, couponId, couponDiscount,
                shoppingCouponCart);
        }
    }
}
