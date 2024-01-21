using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helper.PlacerOrder
{
    public static class CouponMovieRefresher
    {
         
       /* public static Task MovieCouponRefresher(List<Voucher> checkCouponMovie, int couponId,
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
                *//*return BadRequest("Coupon is expires, Your Cart has been refresh. Check the details before you order.");*//*
            }
        }*/
    }
}
