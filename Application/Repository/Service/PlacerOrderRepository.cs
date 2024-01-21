using Application.Repository.Factory.PlaceOrder;
using Application.Repository.IService;
using Application.Repository.IService.SubInterface;
using Application.ViewModel.Movie;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service
{
    public class PlacerOrderRepository : IPlaceOrder
    {
        private readonly ApplicationDbContext _context;
        /*private readonly ISampFactory _sampFactory;*/
         
        public PlacerOrderRepository(ApplicationDbContext context )
        {
            _context = context;
        }
        public async Task AddingProductCustomerNotification(string userId, DateTime now, List<ProductTransaction> productTransaction)
        {
            var productCustomerNotif = (from t in productTransaction
                                        select new ProductTransactionNotif
                                        {
                                            Creation = now,
                                            ProductTransactionId = t.Id,
                                            ProductImage = t.PlaceOrderItems.Movie.ImageURL,
                                            Header = GenerateHeader(t.PlaceOrderItems.Movie.Name),
                                            MessageBody = GenerateMessageBody(t.PlaceOrderItems.UserPlaceOrder.UserName, t.PlaceOrderItems.Vendor.UserName),
                                            ReceivingUserId = userId
                                        }).ToList();
            await _context.ProductTransactionNotif.AddRangeAsync(productCustomerNotif);
        }
        private string GenerateHeader(string name) => $"You have successfully purchased the {name}";
        
        private string GenerateMessageBody(string userName1, string userName2)=> $"Hi {userName1} your purchased product is " +
                $"now prepared by {userName2}"; 
        public async Task<List<ProductTransaction>> AddingProductTransaction(string userId, List<PlaceOrderItems> placeOrder)
        {
            List<ProductTransaction> productTransaction = (from t in placeOrder
                                                           select new ProductTransaction
                                                           {
                                                               ConsumerId = userId,
                                                               VendorId = t.VendorId,
                                                               PlaceOrderItemsId = t.Id,
                                                               OrderPlaced = true,
                                                               PaymentConfirmed = true
                                                           }).ToList();

            await _context.ProductTransactions.AddRangeAsync(productTransaction);
            await _context.SaveChangesAsync();

            return productTransaction;
        }

        public async Task AddingProductTransactionReponse(string VENDOR_SHIP_THE_PARCEL, DateTime now, List<ProductTransaction> productTransaction)
        {
            var productTrans = (from s in productTransaction
                                select new ProductTransactionResponse()
                                {
                                    ProductTransactionId = s.Id,
                                    ResponseCreation = now,
                                    Message = VENDOR_SHIP_THE_PARCEL,
                                }).ToList();

            await _context.ProductTransactionResponses.AddRangeAsync(productTrans);
            await _context.SaveChangesAsync();
        }

        public async Task AddingProductVendorNotification(ApplicationUser user, DateTime now, List<ProductTransaction> productTransaction)
        {
            var productVendorNotif = (from t in productTransaction
                                      select new ProductTransactionNotif
                                      {
                                          Creation = now,
                                          ProductTransactionId = t.Id,
                                          ProductImage = t.PlaceOrderItems.Movie.ImageURL,
                                          Header = $"{user.UserName} has placed an order to your product.",
                                          MessageBody = $"Hi {t.PlaceOrderItems.Vendor.UserName}, {user.UserName} has bought your {t.PlaceOrderItems.Movie.Name} product. ",
                                          ReceivingUserId = t.PlaceOrderItems.VendorId
                                      }).ToList();
            await _context.ProductTransactionNotif.AddRangeAsync(productVendorNotif);
        }

        public async Task CouponMovieRefresher(List<Voucher> checkCouponMovie, int couponId, int couponDiscount, List<ShoppingCartItem> shoppingCouponCart)
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
                /*return BadRequest("Coupon is expires, Your Cart has been refresh. Check the details before you order.");*/
            }
        }

        public async Task VoucherMovieRefresher(List<Voucher> checkVoucherMovie, int voucherxId,
            int voucherDiscount, List<ShoppingCartItem> shoppingVoucherCart)
        {
            if (checkVoucherMovie != null || checkVoucherMovie.Count != 0)
            {
                voucherxId = checkVoucherMovie.FirstOrDefault().Id;

                var shopeeVoucheroucher = _context.ShoppingVouchers
                    .Where(w => w.VoucherId == voucherxId)
                    .ToList();

                voucherDiscount = checkVoucherMovie.FirstOrDefault().DiscountPercentage;

                _context.AttachRange(shoppingVoucherCart);

                shoppingVoucherCart.ForEach(w =>
                {
                    w.IsVoucher = false;
                    w.TotalDiscount -= voucherDiscount;
                });

                _context.ShoppingVouchers.RemoveRange(shopeeVoucheroucher);

                await _context.SaveChangesAsync();
            }
        }
        public async Task DiscountMovieRefresher(List<Voucher> checkCouponMovie,
            int couponId, int couponDiscount, List<ShoppingCartItem> shoppingCouponCart, bool IsVoucherType)
        {
            // IsVoucherType == true its for voucher else its for coupon turning boolean to false.
            if (checkCouponMovie != null && checkCouponMovie.Count > 0)
            {
                couponId = checkCouponMovie.FirstOrDefault().Id;

                var shopeeCouponVoucher = _context.ShoppingVouchers
                .Where(w => w.VoucherId == couponId)
                .ToList();

                couponDiscount = checkCouponMovie.FirstOrDefault().DiscountPercentage;

                _context.AttachRange(shoppingCouponCart);

                if (IsVoucherType == true)
                {
                    shoppingCouponCart.ForEach(w =>
                    {
                        w.IsVoucher = false;
                        w.TotalDiscount -= couponDiscount;
                    });
                }
                else if (IsVoucherType == true)
                {
                    shoppingCouponCart.ForEach(w =>
                    {
                        w.IsCoupon = false;
                        w.TotalDiscount -= couponDiscount;
                    });
                }

                /*shoppingCouponCart.ForEach(w =>
                {
                    w.IsCoupon = false;
                    w.TotalDiscount -= couponDiscount;
                });*/
                _context.ShoppingVouchers.RemoveRange(shopeeCouponVoucher);
                await _context.SaveChangesAsync();
                /*return BadRequest("Coupon is expires, Your Cart has been refresh. Check the details before you order.");*/
            }
        }
        public async Task DecreasingVoucherQuantity(string userId, DateTime now)
        {
            // Make sure all Vochers are valid & its yours.
            var validShoppingVouchers = _context.ShoppingVouchers
                       .Include(w => w.Voucher)
                       .Where(w => w.ShoppingVoucherUserid == userId) // ==
                       .Where(w => w.Voucher.Expire > now || w.Voucher.Quantity > 0)
                       .ToList();

            var voucher = new Voucher();

            var myCart = new ShoppingCartItem();

            var voucherList = _context.Vouchers
                .ToList();

            var myShoppingCart = _context.ShoppingCartItems
                    .Where(w => w.MyCartUserId == userId).ToList();

            var toAddVoucherList = new List<Voucher>();

            foreach (var item in validShoppingVouchers)
            {
                voucher = voucherList.FirstOrDefault(w => w.Id == item.VoucherId);

                myCart = myShoppingCart.FirstOrDefault(w => w.Id == item.ShoppingCartItemId);

                if (myCart.Id == item.ShoppingCartItemId && voucher.Id == item.VoucherId)
                {
                    if (voucher.Quantity > 0)
                    {
                        /* voucher.Quantity--; trrr*/
                        voucher.Quantity = voucher.Quantity - myCart.Amount;

                        toAddVoucherList.Add(voucher);
                    }
                }
            }
            _context.Vouchers.UpdateRange(toAddVoucherList);
            await _context.SaveChangesAsync();
        }

        public async Task DecreasingShopQuantity(MovieCheckout dto, string userId, DateTime now)
        {
            var discoutendShopVendor = await _context.DiscountedShop
                                      .Where(w => w.Expire > now && w.Quantity > 0)
                                      .Select(w => w.VendorId)
                                      .ToListAsync();

            var shoppingCartVendor = await _context.ShoppingCartItems
                .Where(w => discoutendShopVendor.Contains(w.VendorId))
                .Where(w => dto.ShoppingCartId.Contains(w.Id))
                .Where(w => w.IsMinimumQuota == true)
                .Where(w => w.MyCartUserId == userId)
                .ToListAsync();

            var discoutendShop = await _context.DiscountedShop
                .Where(w => shoppingCartVendor.Select(w => w.VendorId).Contains(w.VendorId))
                .Where(w => w.Expire > now && w.Quantity > 0)
                .ToListAsync();

            _context.AttachRange(discoutendShop);
            discoutendShop.ForEach(w =>
            {
                w.Quantity -= shoppingCartVendor.Count;
            });
            await _context.SaveChangesAsync();

        }
        public async Task UserLoginMovieOutOfStockNotify(int movieId, ApplicationUser user, DateTime now, Movie movie)
        {
            var movieOutOfStock = new MovieOutOfStock()
            {
                UserAttemptToCartOutOfStockId = user.Id,
                DateCreation = now,
                MovieId = movieId,
                IsOutOfStock = true
            };
            await _context.MovieOutOfStocks.AddAsync(movieOutOfStock);
            await _context.SaveChangesAsync();

            var movieOutOfStockNotification = new MovieOutOfStockNotification()
            {
                ReceivingUserId = user.Id,
                MovieOutOfStockId = movieOutOfStock.Id,
                Creation = now,
                Header = $"Good Day {user.UserName}! The product you`ve selected is now out of stock.",
                MessageBody = $"The {movie.Name} from {movie.Vendor.UserName} is currently out of stock. We had already notified him to re-stock your selected product. ",
                ProductImage = movie?.ImageURL
            };
            await _context.MovieOutOfStockNotifications.AddAsync(movieOutOfStockNotification);
            await _context.SaveChangesAsync();
        }

        public async Task UserLoginMovieOutOfStockNotifyList(ApplicationUser user, MovieCheckout dto,
            DateTime now)
        {
            var movieOutOfStockToAdd = new List<MovieOutOfStock>();

            var shoppingCart = _context.ShoppingCartItems
                                        .Where(w => w.MyCartUserId == user.Id)
                                        .Where(w => dto.ShoppingCartId.Contains(w.Id))
                                        .ToList();

            foreach (var item in shoppingCart)
            {
                var movieOutOfStock = new MovieOutOfStock()
                {
                    UserAttemptToCartOutOfStockId = user.Id,
                    DateCreation = now,
                    MovieId = item.MovieId,
                    IsOutOfStock = true
                };
                movieOutOfStockToAdd.Add(movieOutOfStock);
            }
 
            await _context.MovieOutOfStocks.AddRangeAsync(movieOutOfStockToAdd);
            await _context.SaveChangesAsync();

            var movieOutOfStockNotificationToAdd = new List<MovieOutOfStockNotification>();
            var movieOutOfStockNotification = new MovieOutOfStockNotification();

            if (movieOutOfStockToAdd.Count != 0)
            {
                foreach (var item in movieOutOfStockToAdd)
                {
                    movieOutOfStockNotification = new MovieOutOfStockNotification()
                    {
                        ReceivingUserId = user.Id,
                        MovieOutOfStockId = item.Id,
                        Creation = now,
                        Header = $"Good Day {user.UserName}! The product you`ve selected is now out of stock.",
                        MessageBody = $"The {item.Movie.Name} from {item.Movie.Vendor.UserName} is currently out of stock. We had already notified him to re-stock your selected product. ",
                        ProductImage = item.Movie?.ImageURL
                    };
                }
                movieOutOfStockNotificationToAdd.Add(movieOutOfStockNotification);

            }

            await _context.MovieOutOfStockNotifications.AddRangeAsync(movieOutOfStockNotificationToAdd);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> MovieOutOfStockUserExistsNotif(string userId, int movieId)
        {
            return await _context.MovieOutOfStocks
                        .Where(w => w.UserAttemptToCartOutOfStockId == userId)
                        .AnyAsync(w => w.MovieId == movieId);
        }

        public async Task<bool> MovieOutOfStockUserExistsNotif(string userId, List<int> movieIds)
        {
            return await _context.MovieOutOfStocks
                        .Where(w => w.UserAttemptToCartOutOfStockId == userId)
                        .Where(w =>movieIds.Contains(w.MovieId))
                        .AnyAsync();
        }
        // all ine
        public async Task AllInProductUserVendorResponse(ApplicationUser user, List<PlaceOrderItems> withAndNonQuotaPlaceOrderList,
            string VENDOR_SHIP_THE_PARCEL, DateTime now)
        {
            var productTransaction = await AddingProductTransaction(user.Id, withAndNonQuotaPlaceOrderList);

            // The notification depends on how many would be the quantity of
            // PlaceOrderItems of the customer
            // Notification for customer
            await AddingProductCustomerNotification(user.Id, now, productTransaction);
            await AddingProductVendorNotification(user, now, productTransaction);
            await AddingProductTransactionReponse(VENDOR_SHIP_THE_PARCEL, now, productTransaction);
        }

        public async Task HandlingCartWithOrWithoutQuota(ApplicationUser user,
            string VENDOR_SHIP_THE_PARCEL, DateTime now,
            int myFinalCartWithoutQuotaCount, int myFinalCartWithQuotaCount,
            List<PlaceOrderItems> placementWithQuotaOrderList,
            List<PlaceOrderItems> placementWithoutQuotaOrderList)
        {
            if (myFinalCartWithQuotaCount != 0 && myFinalCartWithoutQuotaCount != 0)
            {
               var withAndNonQuotaPlaceOrderList = placementWithQuotaOrderList
                    .Concat(placementWithoutQuotaOrderList).ToList();


                /* await _context.PlaceOrderItems.AddRangeAsync(withAndNonQuotaPlaceOrderList);
                await _context.SaveChangesAsync();

                // pptest
                * var  productTransaction = await _placeOrder.AddingProductTransaction(user.Id, withAndNonQuotaPlaceOrderList);

                // The notification depends on how many would be the quantity of
                // PlaceOrderItems of the customer
                // Notification for customer

                //
                //await _placeOrder.AddingProductCustomerNotification(user.Id, now, productTransaction);
                //await _placeOrder.AddingProductVendorNotification(user, now, productTransaction);
                //await _placeOrder.AddingProductTransactionReponse(VENDOR_SHIP_THE_PARCEL, now, productTransaction);
                //
                await AllInProductUserVendorResponse(user, withAndNonQuotaPlaceOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);
                await _context.SaveChangesAsync();*/

                await CartBehavior(user, withAndNonQuotaPlaceOrderList,
                    VENDOR_SHIP_THE_PARCEL,now);

            }
            else if (myFinalCartWithQuotaCount != 0 && myFinalCartWithoutQuotaCount == 0)
            {
                /*await _context.PlaceOrderItems.AddRangeAsync(placementWithQuotaOrderList);
                await _context.SaveChangesAsync();

                await AllInProductUserVendorResponse(user, placementWithQuotaOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);
                await _context.SaveChangesAsync();*/

                await CartBehavior(user, placementWithQuotaOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);
            }
            else if (myFinalCartWithoutQuotaCount != 0 && myFinalCartWithQuotaCount == 0)
            {
                /*await _context.PlaceOrderItems.AddRangeAsync(placementWithoutQuotaOrderList);
                await _context.SaveChangesAsync();

                await AllInProductUserVendorResponse(user, placementWithoutQuotaOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);
                await _context.SaveChangesAsync();*/


                await CartBehavior(user, placementWithoutQuotaOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);
            }

            /*return withAndNonQuotaPlaceOrderList;*/
        }

         
        private async Task CartBehavior(ApplicationUser user,
           List<PlaceOrderItems> withAndNonQuotaPlaceOrderList,
           string VENDOR_SHIP_THE_PARCEL, DateTime now)
        {
            await _context.PlaceOrderItems.AddRangeAsync(withAndNonQuotaPlaceOrderList);
            await _context.SaveChangesAsync();

            await AllInProductUserVendorResponse(user, withAndNonQuotaPlaceOrderList,
                VENDOR_SHIP_THE_PARCEL, now);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsInMovieOutOfStock(ApplicationUser user, MovieCheckout dto)
        {
            var shoppingCart = _context.ShoppingCartItems
                                         .Where(w => w.MyCartUserId == user.Id)
                                         .Where(w => dto.ShoppingCartId.Contains(w.Id))
                                         .ToList();
            return await MovieOutOfStockUserExistsNotif(user.Id,
                                                    shoppingCart.Select(w => w.MovieId).ToList());
        }

        public void AddingMovieToChangeNonQuotav2(Movie movieToChangeNonQuota, List<Movie> movieToChangeListNonQuota, int exactAmount, int StockCountResult)
        {
            movieToChangeNonQuota.Sold += exactAmount;
            movieToChangeNonQuota.StockCount = StockCountResult;

            movieToChangeListNonQuota.Add(movieToChangeNonQuota);
        }

        public void AddingMovieToChangeWithQuotav2(Movie movieToChange, List<Movie> movieToChangeList, int exactAmount, int StockCountResult)
        {
            movieToChange.Sold += exactAmount;
            movieToChange.StockCount = StockCountResult;
            movieToChangeList.Add(movieToChange);
        }

        public void AddingMovieToChangeNonQuota(Movie movieToChangeNonQuota, List<Movie> movieToChangeListNonQuota, int soldCount, int stockCount)
        {
            movieToChangeNonQuota.Sold += soldCount;
            movieToChangeNonQuota.StockCount -= stockCount;

            movieToChangeListNonQuota.Add(movieToChangeNonQuota);
        }

        public void AddingMovieToChangeWithQuota(Movie movieToChangeNonQuota, List<Movie> movieToChangeListNonQuota, int soldCount, int stockCount)
        {
            movieToChangeNonQuota.Sold += soldCount;
            movieToChangeNonQuota.StockCount -= stockCount;

            movieToChangeListNonQuota.Add(movieToChangeNonQuota);
        }

        public PlaceOrderItems AddValidAmountOrder(ApplicationUser user, DateTime now, List<PlaceOrderItems> placementWithQuotaOrderList, ShoppingCartItem item, int validAmount, decimal getAllPercentage, double totalOriginalPrice)
        {
            var placementWithQuotaOrder = new PlaceOrderItems()
            {
                Price = item.Movie.Price,
                TotalDiscount = getAllPercentage,
                MovieId = item.MovieId,
                Amount = validAmount,
                PlacedOrderCreation = now,
                UserPlaceOrderId = user.Id,
                ShippingFee = item.ShippingFee,
                VendorId = item.VendorId,
                TotalPrice = totalOriginalPrice + item.ShippingFee
            };
            //
            placementWithQuotaOrderList.Add(placementWithQuotaOrder);

            return placementWithQuotaOrder;
        }

        public PlaceOrderItems AddWholeAmountOrder(string userId, DateTime now, List<PlaceOrderItems> placementWithQuotaOrderList, ShoppingCartItem item, decimal getAllPercentage, double totalOriginalPrice)
        {
            var placementWithQuotaOrder = new PlaceOrderItems()
            {
                Price = item.Movie.Price,
                TotalDiscount = getAllPercentage,
                MovieId = item.MovieId,
                Amount = item.Amount,
                PlacedOrderCreation = now,
                UserPlaceOrderId = userId,
                ShippingFee = item.ShippingFee,
                VendorId = item.VendorId,
                TotalPrice = totalOriginalPrice + item.ShippingFee
            };
            //
            placementWithQuotaOrderList.Add(placementWithQuotaOrder);
            return placementWithQuotaOrder;

        }
        public PlaceOrderItems AddWholeAmountWithoutQuotaOrder(string userId, DateTime now,
            List<PlaceOrderItems> placementWithoutQuotaOrderList, ShoppingCartItem item,
            decimal getAllPercentage, double totalOriginalPrice)
        {
            var placementWithQuotaOrder = new PlaceOrderItems()
            {
                Price = item.Movie.Price,
                TotalDiscount = getAllPercentage,
                MovieId = item.MovieId,
                Amount = item.Amount,
                PlacedOrderCreation = now,
                UserPlaceOrderId = userId,
                ShippingFee = item.ShippingFee,
                VendorId = item.VendorId,
                TotalPrice = totalOriginalPrice + item.ShippingFee
            };
            //
            placementWithoutQuotaOrderList.Add(placementWithQuotaOrder);
            return placementWithQuotaOrder;

        }
        public (decimal getAllPercentage, double totalOriginalPrice) GeneratedReachedPriceQuota(DiscountShop_Cart shopDiscount, ShoppingCartItem item, int typeAmount)
        {
            decimal promotionPercentage = 100 *
                ((decimal)(shopDiscount.DiscountedShop.FixedDiscount / item.Price));

            var getAllPercentage = item.TotalDiscount + promotionPercentage;

            var sumOrigPriceWithAmount = item.Movie.Price * typeAmount;

            var totalPriceDiscountDeduction = sumOrigPriceWithAmount * ((double)getAllPercentage / 100);

            var totalOriginalPrice = sumOrigPriceWithAmount - totalPriceDiscountDeduction;

            return (getAllPercentage, totalOriginalPrice);
        }
        public (decimal getAllPercentage, double totalOriginalPrice) GeneratedReachedWithoutPriceQuota(
            ShoppingCartItem item, int typeAmount)
        {
            var getAllPercentage = item.TotalDiscount;

            var sumOrigPriceWithAmount = item.Movie.Price * typeAmount;

            var totalPriceDiscountDeduction = sumOrigPriceWithAmount * ((double)getAllPercentage / 100);

            var totalOriginalPrice = sumOrigPriceWithAmount - totalPriceDiscountDeduction;

            return (getAllPercentage, totalOriginalPrice);

        }
        public (decimal getAllPercentage, double totalOriginalPrice) GeneratedNonReachedPriceQuota(ShoppingCartItem item, int typeAmount)
        {
            var getAllPercentage = item.TotalDiscount;

            var sumOrigPriceWithAmount = item.Movie.Price * typeAmount;

            var totalPriceDiscountDeduction = sumOrigPriceWithAmount * ((double)getAllPercentage / 100);

            var totalOriginalPrice = sumOrigPriceWithAmount - totalPriceDiscountDeduction;

            return (getAllPercentage, totalOriginalPrice);
        }
        public (decimal getAllPercentage, double totalOriginalPrice) GeneratedNonReachedWithoutPriceQuota(
            ShoppingCartItem item, int typeAmount)
        {
            var getAllPercentage = item.TotalDiscount;

            var sumOrigPriceWithAmount = item.Movie.Price * typeAmount;

            var totalPriceDiscountDeduction = sumOrigPriceWithAmount * ((double)getAllPercentage / 100);

            var totalOriginalPrice = sumOrigPriceWithAmount - totalPriceDiscountDeduction;

            return (getAllPercentage, totalOriginalPrice);
        }
        public async Task<List<ShoppingCartItem>> MyShoppingCartList(MovieCheckout dto, string userId, bool haveQuotaPrice)
        {
            return await _context.ShoppingCartItems
                                .Include(w => w.Movie)
                                .Include(w => w.Movie).ThenInclude(w => w.Vendor)
                                .Where(w => w.IsSelected == true && w.IsMinimumQuota == haveQuotaPrice)
                                .Where(w => w.MyCartUserId == userId)
                                .Where(w => dto.ShoppingCartId.Contains(w.Id))
                                .ToListAsync();
        }

        public PlaceOrderItems AddValidAmountWithoutQuotaOrder(string userId, DateTime now, List<PlaceOrderItems> placementWithoutQuotaOrderList, ShoppingCartItem item, int validAmount, decimal getAllPercentage, double totalOriginalPrice)
        {
            var placementWithQuotaOrder = new PlaceOrderItems()
            {
                Price = item.Movie.Price,
                TotalDiscount = getAllPercentage,
                MovieId = item.MovieId,
                Amount = validAmount,
                PlacedOrderCreation = now,
                UserPlaceOrderId = userId,
                ShippingFee = item.ShippingFee,
                VendorId = item.VendorId,
                TotalPrice = totalOriginalPrice + item.ShippingFee
            };
            //
            placementWithoutQuotaOrderList.Add(placementWithQuotaOrder);
            return placementWithQuotaOrder;
        }

        public DiscountShop_Cart ShopDiscount(string MovieVendorId)
        {
            var shopDiscountedList = ShopDiscountedList();
            return  shopDiscountedList.FirstOrDefault(w => w.DiscountedShop.VendorId == MovieVendorId);

            /*shopDiscount = shopDiscountedList.FirstOrDefault(w => w.DiscountedShop.VendorId == item.Movie.VendorId);*/

        }

        public  List<DiscountShop_Cart> ShopDiscountedList()
        {
            
            return _context.DiscountShop_Cart
                           .Include(w => w.DiscountedShop)
                           .ToList();
        }

        public double ReachedPriceQuota(Movie movieSelected, ShoppingCartItem item, int validAmount)
        {
            return item.Amount > movieSelected.StockCount ? validAmount * item.Price
                                               : item.Amount <= movieSelected.StockCount ? item.Amount * item.Price : 0;
        }

        public bool ReachedPriceQuotaMinimumSpend(double reachedPriceQuota, string movieVendorId)
        {
            var shopDiscount = ShopDiscount(movieVendorId);
            return reachedPriceQuota >= shopDiscount.DiscountedShop.MinimumSpend;
        }
    }
}
