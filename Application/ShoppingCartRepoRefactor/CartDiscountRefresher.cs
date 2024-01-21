using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.ShoppingCartRepoRefactor
{
    public class CartDiscountRefresher
    {
        private void CartRefresh(List<ShoppingCartItem> allCart, List<ShoppingVoucher> shoppingCartVoucher, List<ShoppingCartItem> cartToAddList, List<Voucher> myVoucherNotAvailable, Voucher myVoucher,
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

                    if (cart.MovieId == myVoucher.MovieId && cart.IsCoupon == true 
                        && cart.IsVoucher == false && myVoucher.Code != null)
                    {
                        // Coupon
                        cart.IsCoupon = false;
                        cart.TotalDiscount -= myVoucher.DiscountPercentage;
                    }
                    else if (cart.MovieId == myVoucher.MovieId && cart.IsVoucher == true 
                        && cart.IsCoupon == false&& myVoucher.Code == null)
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
    }
}
