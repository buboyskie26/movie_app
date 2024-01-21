using Application.Repository.IService;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Strategy.PlaceOrder.Samp
{
    public interface ISampHead
    {
        public (decimal getAllPercentage, double totalOriginalPrice) IGeneratedReachedPriceQuota(DiscountShop_Cart shopDiscount,
            ShoppingCartItem item, int typeAmount);

        public bool ReachedPriceQuotaMinimumSpend(double reachedPriceQuota, string movieVendorId);

    }
    public class SampClass : ISampHead
    {

        private readonly IPlaceOrder _placeOrder;

        public SampClass(IPlaceOrder placeOrder)
        {
            _placeOrder = placeOrder;
        }

        public (decimal getAllPercentage, double totalOriginalPrice) IGeneratedReachedPriceQuota(DiscountShop_Cart shopDiscount,
            ShoppingCartItem item, int typeAmount)
        {

            decimal promotionPercentage = 100 *
              ((decimal)(shopDiscount.DiscountedShop.FixedDiscount / item.Price));

            var getAllPercentage = item.TotalDiscount + promotionPercentage;

            var sumOrigPriceWithAmount = item.Movie.Price * typeAmount;

            var totalPriceDiscountDeduction = sumOrigPriceWithAmount * ((double)getAllPercentage / 100);

            var totalOriginalPrice = sumOrigPriceWithAmount - totalPriceDiscountDeduction;

            return (getAllPercentage, totalOriginalPrice);

        }

        public bool ReachedPriceQuotaMinimumSpend(double reachedPriceQuota, string movieVendorId)
        {
            var shopDiscount = _placeOrder.ShopDiscount(movieVendorId);
            return reachedPriceQuota >= shopDiscount.DiscountedShop.MinimumSpend;


        }
    }
}
