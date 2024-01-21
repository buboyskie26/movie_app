using Application.Repository.IService.SubInterface;
using Application.ViewModel.Movie;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService
{
    public interface IPlaceOrder : IShoppingCartRefresher, IOutOfStockNotification        
    {
        Task<List<ProductTransaction>> AddingProductTransaction(string userId,
            List<PlaceOrderItems> placeOrder);
        Task AddingProductCustomerNotification(string userId, DateTime now,
            List<ProductTransaction> productTransaction);
        Task AddingProductVendorNotification(ApplicationUser user, DateTime now,
            List<ProductTransaction> productTransaction);
        Task AddingProductTransactionReponse(string VENDOR_SHIP_THE_PARCEL, DateTime now,
            List<ProductTransaction> productTransaction);
        Task DecreasingVoucherQuantity(string userId, DateTime now);
        Task DecreasingShopQuantity(MovieCheckout dto, string userId, DateTime now);
        Task UserLoginMovieOutOfStockNotifyList(
            ApplicationUser user, MovieCheckout dto, DateTime now);
        Task DiscountMovieRefresher(List<Voucher> checkCouponMovie, int couponId, int couponDiscount,
           List<ShoppingCartItem> shoppingCouponCart, bool IsVoucherType);
        Task AllInProductUserVendorResponse(ApplicationUser user, List<PlaceOrderItems> withAndNonQuotaPlaceOrderList,
            string VENDOR_SHIP_THE_PARCEL, DateTime now);

        Task HandlingCartWithOrWithoutQuota(ApplicationUser user, string VENDOR_SHIP_THE_PARCEL,
            DateTime now, int myFinalCartWithoutQuotaCount, int myFinalCartWithQuotaCount,
            List<PlaceOrderItems> placementWithQuotaOrderList,
            List<PlaceOrderItems> placementWithoutQuotaOrderList);

        Task<bool> UserExistsInMovieOutOfStock(ApplicationUser user, MovieCheckout dto);
        void AddingMovieToChangeNonQuotav2(Movie movieToChangeNonQuota, List<Movie> movieToChangeListNonQuota, int exactAmount, int StockCountResult);
        void AddingMovieToChangeWithQuotav2(Movie movieToChange, List<Movie> movieToChangeList, int exactAmount, int StockCountResult);

        void AddingMovieToChangeNonQuota(Movie movieToChangeNonQuota, List<Movie> movieToChangeListNonQuota,
            int soldCount, int stockCount);
        void AddingMovieToChangeWithQuota(Movie movieToChangeNonQuota, List<Movie> movieToChangeListNonQuota,
            int soldCount, int stockCount);
        PlaceOrderItems AddValidAmountOrder(ApplicationUser user, DateTime now, List<PlaceOrderItems> placementWithQuotaOrderList,
            ShoppingCartItem item, int validAmount, decimal getAllPercentage, double totalOriginalPrice);

        PlaceOrderItems AddWholeAmountOrder(string userId, DateTime now,
            List<PlaceOrderItems> placementWithQuotaOrderList, ShoppingCartItem item,
            decimal getAllPercentage, double totalOriginalPrice);

        PlaceOrderItems AddWholeAmountWithoutQuotaOrder(string userId, DateTime now, List<PlaceOrderItems> placementWithoutQuotaOrderList, ShoppingCartItem item,
            decimal getAllPercentage, double totalOriginalPrice);

        PlaceOrderItems AddValidAmountWithoutQuotaOrder(string userId, DateTime now, List<PlaceOrderItems> placementWithoutQuotaOrderList,
            ShoppingCartItem item, int validAmount,
      decimal getAllPercentage, double totalOriginalPrice);

        (decimal getAllPercentage, double totalOriginalPrice) GeneratedReachedPriceQuota(DiscountShop_Cart shopDiscount,
            ShoppingCartItem item, int typeAmount);
        (decimal getAllPercentage, double totalOriginalPrice) GeneratedReachedWithoutPriceQuota(
     ShoppingCartItem item, int typeAmount);
        (decimal getAllPercentage, double totalOriginalPrice) GeneratedNonReachedPriceQuota(
          ShoppingCartItem item, int typeAmount);

        (decimal getAllPercentage, double totalOriginalPrice) GeneratedNonReachedWithoutPriceQuota(
                 ShoppingCartItem item, int typeAmount);

        Task<List<ShoppingCartItem>> MyShoppingCartList(MovieCheckout dto, string userId, bool haveQuotaPrice);

        //

        DiscountShop_Cart ShopDiscount(string MovieVendorId);
        List<DiscountShop_Cart> ShopDiscountedList();
        double ReachedPriceQuota(Movie movieSelected, ShoppingCartItem item, int validAmount);
        public bool ReachedPriceQuotaMinimumSpend(double reachedPriceQuota, string movieVendorId);

    }
}
