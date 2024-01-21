using Application.Repository.Service.SampOCP;
using Application.Repository.Service.ShoppingProcessor.Interface.SubInterface;
using Application.ViewModel.ShoppingCart;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service.ShoppingProcessor.Interface
{
    public interface IShoppingDiscountHall : IAddingShoppingCartBehavior,
        IDiscountExpirationBehavior
    {
        ShoppingCartItem AddingSamp(ISampAddingProp sampAddingProp);
        void PriceQuotaAndCouponAdjustment(bool IsCoupon,bool IsTotalDiscountAdd,
            bool IsMinimumQuota, ShoppingCartItem couponWithVoucherCart,Voucher couponValueCart);

        int HalfPriceShippingFee(int firstHalf, int secondHalf, DateTime now, int movieShipping);

    }

}
