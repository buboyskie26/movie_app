using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repository.Service.ShoppingProcessor.Interface.SubInterface
{
    public interface IDiscountBehavior
    {
        decimal CouponTotalDiscountPrice(List<Voucher> couponList, Movie movie, ref decimal totalCouponDiscount);
        decimal TotalDiscountedPrice(Movie movie, decimal totalDiscount);
        decimal VoucherTotalDiscountPrice(List<Voucher> voucherList, Movie movie,
            ref decimal totalVoucherDiscount);
        decimal CouponAndVoucherDiscountedPrice(Movie movie, decimal finalPrice, decimal totalVoucherDiscount, decimal totalCouponDiscount);
        decimal VoucherCouponTotalDiscount(List<Voucher> voucherList, List<Voucher> couponList, ref decimal totalVoucherDiscount, ref decimal totalCouponDiscount);
    }
}
