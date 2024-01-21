using Application.Helper;
using Application.Repository.Factory.Movie;
using Application.Repository.IService;
using Application.ViewModel.Movie;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private string container = "movies";
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMessageNotification _messageNotification;
        private readonly IMovieBehavior _movieBehavior;


        public MovieController(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IMessageNotification messageNotification, IMovieBehavior movieBehavior)
        {
            _context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            _userManager = userManager;
            _signInManager = signInManager;
            _messageNotification = messageNotification;
            _movieBehavior = movieBehavior;
        }

        [HttpGet("MovieDashboard")]
        public async Task<ActionResult<MovieProductView>> MovieDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var now = DateTime.Now;
            // Customer surely used this page more often just to find other products, 
            // Customer added some item to their cart and look onto this page
            // It will generate a message from vendor, remembering that there`s something product in their cart
            
            var shoppingCart = _context.ShoppingCartItems
                .Include(w => w.Movie)
                .Where(w => w.MyCartUserId == user.Id)
                .ToList();

            var spanTime = shoppingCart.FirstOrDefault().DateAddToCart;
             
            /*var messageTableList = new List<MessageTable>();*/

      

            // Check if the messageUserTable has a movie Id to prevent duplication.
            var userTables = await _context.MessageTables
                .Where(w=> shoppingCart.Select(t=> t.MovieId).Contains(w.MovieRefId))
                .AnyAsync(w=> w.UserHeHadMessageId == user.Id);

            if (userTables == false)
            {
                // Check if the soanTime exceeds by the current date
                if (now >= spanTime.AddSeconds(5))
                {
                    var (messageTable, messageTableList) = await _messageNotification.AddingProductMessageTable(now, shoppingCart);
                    await _messageNotification.AddProductMessageUserTable(now, messageTable, messageTableList);
                }
            }

            /*var messageToCartUser = */

            var movieProducts = await (from m in _context.Movies
                             select new MovieProduct()
                             {
                                 MovieDescription = m.Description,
                                 MovieId = m.Id,
                                 MovieName = m.Name,
                                 SoldCount = m.Sold,
                                 Address = m.Address,
                                 StockCount = m.StockCount,
                                 SoldItem = m.Sold,
                                 MoviePicture=m.ImageURL
                                 // rating must have 1-5 depends on the sum of user rating review.
                             }).AsNoTracking().ToListAsync();

            var trendingSearch = await (from t in _context.MovieUserViews
                                        select new TrendingMovieSearches
                                        {
                                            MovieId= t.MovieId,
                                            MovieName=t.Movie.Name,
                                            DateViewed = t.ViewDate,
                                            MoviePicture=t.Movie.ImageURL,
                                            VendorName = t.Movie.Vendor.FirstName+" "+ t.Movie.Vendor.FirstName
                                        }).AsNoTracking().OrderByDescending(w=> w.DateViewed).Take(5).ToListAsync();

            var myMovieViews = _context.MovieUserViews
                .Include(w => w.Movie)
                .Where(w => w.UserWhoViewId == user.Id)
                .Select(w => w.Movie.ProductCategory)
                .ToList();

            var allMovies = await _context.Movies.Include(w=> w.Vendor).ToListAsync();
            var movie = new Movie();
            var movieList = new List<Movie>();

            foreach (var item in allMovies)
            {
                if (myMovieViews.Contains(item.ProductCategory) == true)
                {
                    // In each  vendor that has similar product Category (most sold) should result only once
                    movieList.Add(item);
                }

            }

            // if the customer viewed the specific movie. the movie product category would be his/her recommendation.
            // if he viewed Laptop & Computers, other stores that sells Laptop & Computers with high sold would be 
            // his recommendation
            // In each  vendor that has similar product Category (most sold) should result only once
            // Once you viewed Laptop & Computers in the specific vendor
            // That vendor most sold specific product category,
            // and other vendor that has Laptop & Computers must show only once
            var recommendProducts = new List<RecommendedProduct>();

            if (movieList != null)
            {
                recommendProducts = (from m in movieList
                       select new RecommendedProduct
                       {
                           MovieId = m.Id,
                           MovieName = m.Name,
                           MoviePicture = m.ImageURL,
                           ProductCategory = m.ProductCategory,
                           VendorName = m.Vendor.UserName,
                           VendorId = m.VendorId,
                           Sold = m.Sold
                       }).ToList();

                /*mov = mov.GroupBy(w => w.VendorId).First().ToList();*/
                recommendProducts = recommendProducts.OrderByDescending(w => w.Sold).DistinctBy(w => new { w.VendorId, w.ProductCategory }).ToList();
            }

            // Flash Deals
            // It has a time limit for the deals
            var flashDeals = await (from m in _context.Vouchers
                                    where m.Expire < DateTime.Now
                                    select new FlashSale
                                    {
                                        Discount = m.DiscountPercentage,
                                        MovieId = m.MovieId,
                                        VoucherId = m.Id,
                                        MovieName = m.Movie.Name,
                                        MoviePicture = m.Movie.ImageURL,
                                        Sold = m.Movie.Sold,
                                        OriginalPrice= m.Movie.Price,
                                        /*OriginalPrice - (OriginalPrice * ((double)Discount / (double)100m))*/
                                        DiscountedPrice = Math.Round(m.Movie.Price - (m.Movie.Price * (m.DiscountPercentage / (double)100m)), 2)
                                    }).AsNoTracking().ToListAsync();
            flashDeals = flashDeals.DistinctBy(w => w.MovieId).ToList();



            var voucherNotifExpireId = _context.VoucherNotifications
               .Include(w => w.Voucher)
               .Select(w => w.VoucherId)
               .ToList();
            // Voucher would start alert the vendor if the expiration date is about 3(hours) to be expires.
            var voucherToExpire = _context.Vouchers.Include(w=> w.Movie)
                .Where(w=> voucherNotifExpireId.Contains(w.Id) == false)
                .ToList();

            var voucherToAdd = new List<Voucher>();

            var voucherNotifExpire = _context.VoucherNotifications.Include(w => w.Voucher)
             .ToList();
            var vouch = new VoucherNotification();
            int subtractCurrentTimeHour = 7;
            foreach (var item in voucherToExpire)
            {
                // means no duplication could be found.
                vouch = voucherNotifExpire.FirstOrDefault(w => w.VoucherId == item.Id);

                if(vouch == null)
                {
                    // 3hours span time for voucher expiration notification.
                    var x = item.Expire.AddHours(subtractCurrentTimeHour);
                    // Check if the voucher expire notification is in the database.
                    // x = 12, now = 9, -3, =9
                    if (now >= x)
                    {
                        // The voucher is about to expire in 3hours
                        voucherToAdd.Add(item);
                    }
                }
                else
                {
                    // Already in the db.
                }
               
            }

            if (voucherToAdd != null)
            {
                var toExpire = (from e in voucherToAdd
                                select new VoucherNotification
                                {
                                    Creation = now,
                                    Header = $"Good Day! The voucher you setted is about to expire {subtractCurrentTimeHour} hours from now",
                                    MessageBody = $"{e.Movie.ImageURL} voucher {e.Id} is about to expire.",
                                    ProductImage = e.Movie.ImageURL,
                                    VoucherId = e.Id,
                                    ReceivingUserId = e.VendorId
                                }).ToList();

                await _context.VoucherNotifications.AddRangeAsync(toExpire);
                /*await _context.SaveChangesAsync();*/
            }
          

            return new MovieProductView
            {
                MovieProductViews = movieProducts,
                TrendingMovieSearch = trendingSearch,
                RecommendedProducts = recommendProducts,
                FlashSales = flashDeals
            };
        }

        private async Task AddProductMessageUserTable(DateTime now, MessageTable messageTable, List<MessageTable> messageTableList)
        {
            var vendorMessageGenerated = new MessageUsersTable();
            var vendorMessageGeneratedList = new List<MessageUsersTable>();

            foreach (var item in messageTableList)
            {
                if (messageTable != null)
                {
                    vendorMessageGenerated = new MessageUsersTable
                    {
                        IsRead = false,
                        MessageCreated = now,
                        MessageTableId = messageTable.Id,
                        UserOneId = item.UserWhoStartMessageId,
                        UserTwoId = item.UserHeHadMessageId,
                        Message = "Did you forget something?An item in your cart is selling out," +
                        " complete your purchase now!"
                    };
                }
                vendorMessageGeneratedList.Add(vendorMessageGenerated);
            }

            await _context.MessageUsersTables.AddRangeAsync(vendorMessageGeneratedList);
            await _context.SaveChangesAsync();
            /*return vendorMessageGenerated;*/
        }

        private async Task<Tuple<MessageTable, List<MessageTable>>> AddingProductMessageTable(DateTime now, List<ShoppingCartItem> shoppingCart)
        {
            var messageTable = new MessageTable();
            var messageTableList = new List<MessageTable>();

            foreach (var item in shoppingCart)
            {
                messageTable = new MessageTable()
                {
                    UserHeHadMessageId = item.MyCartUserId,
                    MessageCreated = now,
                    UserWhoStartMessageId = item.Movie.VendorId,
                    MovieRefId = item.MovieId
                };
                messageTableList.Add(messageTable);
            }
            await _context.MessageTables.AddRangeAsync(messageTableList);
            await _context.SaveChangesAsync();

           /* var values = new Tuple<string, int>(product.Name, voucher.DiscountPercentage);*/
            var values = new Tuple<MessageTable, List<MessageTable>>(messageTable, messageTableList);
            return await Task.FromResult(values);
            /*return messageTable;*/
        }

        [HttpGet("SingleMovie")]
        [AllowAnonymous]
        public async Task<ActionResult<MovieDashboardView>> GetSingleMovie([FromQuery] SingleMovieView dto)
        {
            var productVouchers = new List<ProductVouchers>();

            // Not signed in
            // Delete all movie out of stock notification that is now available.
            await RemovingOutOfStockThatsNowAvailable();
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

                var isAlreadyViewed = await _context.MovieUserViews
                    .Include(w => w.UserWhoView)
                    .Where(w => w.UserWhoView.Role == "User")
                    .Where(w => w.UserWhoViewId == user.Id)
                    .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

                // delete all voucher that is now expires;

                var voucherExpiresList = await _context.Vouchers
                    .Where(w => w.MovieId == dto.MovieId)
                    .Where(w => w.Expire < DateTime.Now)
                    .ToListAsync();

                var movieOutOfStock = await _context.MovieOutOfStocks
                .Where(w => w.UserAttemptToCartOutOfStockId == user.Id)
                /*.Where(w=> w.IsOutOfStock == false)*/
                .FirstOrDefaultAsync(w => w.MovieId == dto.MovieId);

                var now = DateTime.Now;

                // movie out o f stock = click true
                // movie available notif - delete.

                // Equivalent..
                // pptest 

                /*await _movieBehavior.MovieBehavior(movieOutOfStock != null,
                    movieOutOfStock.IsOutOfStock == true, user.Id, dto.MovieId);*/

                // Equivalent..
                if (movieOutOfStock != null && movieOutOfStock.IsOutOfStock == true)
                {
                    var dateClick = movieOutOfStock?.DateCreation;
                    // once you clicked, it will redirect to product itself
                    movieOutOfStock.IsClicked = true;

                    /*context.MovieOutOfStocks.Remove(movieOutOfStock);*/
                    await _context.SaveChangesAsync();

                    /*return Ok("Movie out of stock clicked.");*/

                }
                else if (movieOutOfStock != null && movieOutOfStock.IsOutOfStock == false)
                {       
                    _context.MovieOutOfStocks.Remove(movieOutOfStock);
                    await _context.SaveChangesAsync();
                    /*return Ok("Movie out of stock notification is now deleted.");*/
                }


                if (voucherExpiresList != null)
                {
                    _context.Vouchers.RemoveRange(voucherExpiresList);
                }

                var movieUsers = new MovieUserView();

                if (isAlreadyViewed == null)
                {
                    movieUsers = new MovieUserView()
                    {
                        MovieId = dto.MovieId,
                        UserWhoViewId = user.Id,
                        ViewDate = DateTime.Now,
                        ViewedTimes = 1,
                    };

                    await _context.MovieUserViews.AddAsync(movieUsers);
                }
                else if (isAlreadyViewed != null)
                {
                    isAlreadyViewed.ViewedTimes += 1;
                    _context.MovieUserViews.Update(isAlreadyViewed);

                }

                productVouchers = await (from r in _context.Vouchers
                                         where r.MovieId == dto.MovieId
                                         where r.Expire >= DateTime.Now && r.Quantity > 0

                                         select new ProductVouchers()
                                         {
                                             MovieId = r.MovieId,
                                             VoucherId = r.Id,
                                             DiscountPrice = r.DiscountPercentage,
                                         }).AsNoTracking().ToListAsync();

                await _context.SaveChangesAsync();
            }

            var movieProduct = await (from m in _context.Movies

                                      select new MovieProduct()
                                      {
                                          MovieDescription = m.Description,
                                          MovieId = m.Id,
                                          MovieName = m.Name,
                                          SoldCount = m.Sold,
                                          Address = m.Address,
                                          StockCount = m.StockCount,
                                          SoldItem = m.Sold,
                                      }).AsNoTracking().FirstOrDefaultAsync();

            var rateProductQuery = _context.RateProducts.AsQueryable();

            if (dto.RateCount != 0)
            {
                rateProductQuery = rateProductQuery.Where(w => w.RateCount == dto.RateCount);
            }
            // If true, All reviews that contains a rate with Images
            // else, All Reviews with/without images.
            if (dto.WithMedia)
            {
                rateProductQuery = rateProductQuery.Where(w => w.ImageUrl != null);
            }

            var rat = await _context.RateProducts
                .Where(w => w.MovieId == dto.MovieId).ToListAsync();
            var s5 = 0;
            var s4 = 0;
            var s3 = 0;
            var s2 = 0;
            var s1 = 0;

            foreach (var item in rat)
            {
                if (item.RateCount == 5)
                    s5 = item.RateCount;
                if (item.RateCount == 4)
                    s4 = item.RateCount;
                if (item.RateCount == 3)
                    s3 = item.RateCount;
                if (item.RateCount == 2)
                    s2 = item.RateCount;
                if (item.RateCount == 1)
                    s1 = item.RateCount;
            }
            double rating = (double)(5 * s5 + 4 * s4 + 3 * s3 + 2 * s2 + 1 * s1) / (s1 + s2 + s3 + s4 + s5);

            rating = Math.Round(rating, 1);

            var userReviews = await (from r in rateProductQuery
                                     where r.MovieId == dto.MovieId
                                     select new ProductRateReview()
                                     {
                                         DateCreated = r.RateCreation.ToString("dd/M/yyyy HH:mm tt", CultureInfo.InvariantCulture),

                                         RateCount = r.RateCount,
                                         Username = r.UserWhoHadRate.FirstName + " " + r.UserWhoHadRate.LastName,
                                         ImageUrl = r.ImageUrl,
                                         Message = r.Message,
                                         MovieId = r.MovieId
                                     })
                                    .AsNoTracking().Paginate(dto.PaginationDTO).ToListAsync();



            await HttpContext.InsertParametersPaginationInHeader(rateProductQuery);



            return new MovieDashboardView
            {
                MovieProducts = movieProduct,
                ProductRateReviews = userReviews,
                OverallRating = rat.Where(w => w.MovieId == dto.MovieId).Any() ? rating : 0,
                ProductVoucher = productVouchers
            };
        }

        private async Task RemovingOutOfStockThatsNowAvailable()
        {
            var nowAvailableProduct = await _context.MovieOutOfStocks
                .Where(w => w.IsOutOfStock == false)
                .ToListAsync();
            if (nowAvailableProduct != null && nowAvailableProduct.Count != 0)
            {
                _context.MovieOutOfStocks.RemoveRange(nowAvailableProduct);
                await _context.SaveChangesAsync();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult> Create([FromForm] MovieCreationDTO gt)
        {
            var user = await _userManager.GetUserAsync(User);
            var now = DateTime.Now;
            var obj = new Movie()
            {
                StartDate = now,
                EndDate = now.AddDays(2),
                Price = gt.Price,
                Description = gt.Description,
                Name = gt.Name,
                Address = user.Address,
                VendorId = user.Id,
                StockCount = gt.Stocks,
                ProductCategory=gt.ProductCategory,
                ShippingFee=gt.ShippingFee,
            };
            obj.TotalStockCount += gt.Stocks;

            if (gt.ImageURL != null)
                obj.ImageURL = await fileStorageService.SaveFile(container, gt.ImageURL);

            await _context.Movies.AddAsync(obj);
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPut("UpdateStock")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult> UpdateStockCount([FromBody] MovieStockDTO gt)
        {
            var user = await _userManager.GetUserAsync(User);

            var now = DateTime.Now;

            var movieExists = await _context.Movies.AnyAsync(w => w.Id == gt.MovieId);

            var movie = await _context.Movies.FirstOrDefaultAsync(w => w.Id == gt.MovieId);
        
            var checkIfMovieOwnedByVendor = _context.Movies.Where(w=> w.VendorId == user.Id)
                .Any(w => w.Id == gt.MovieId);

            if (movieExists == false)
                return BadRequest("Movie Id doesnt exists");

            if (checkIfMovieOwnedByVendor == false)
                return BadRequest("Only your product is updatable.");

            if (movie != null && checkIfMovieOwnedByVendor == true)
            {
                movie.StockCount += gt.StockNumber;
                movie.TotalStockCount += gt.StockNumber;

                _context.Movies.Update(movie);
                await _context.SaveChangesAsync();

                // Todo if the stock is now > 0, the user who attempt to shoppingCart the product 
                // will be having a notification which is a indication that the product is now available.
                // All the data related to the id of the product which indicates the product is not available
                // that data would be replaced by the product is now available.
                var userToNotify = await _context.MovieOutOfStocks
                    .Include(w => w.UserAttemptToCartOutOfStock)
                    .Include(w => w.Movie).ThenInclude(w=> w.Vendor)
                    .Where(w=> w.MovieId == gt.MovieId && w.IsOutOfStock == true)
                    .Select(w=> w.UserAttemptToCartOutOfStockId)
                    .ToListAsync();

                var userToNotifyReference = userToNotify;

                var outOfStockToDelete = await _context.MovieOutOfStocks
                    .Where(w=> userToNotify.Contains(w.UserAttemptToCartOutOfStockId))
                    .ToListAsync();
                // If updating the stock, The Out of stock notification will be removed
                // and replaced by 'is now available'
                if(outOfStockToDelete!=null && outOfStockToDelete.Count != 0)
                {
                    _context.MovieOutOfStocks.RemoveRange(outOfStockToDelete);
                    await _context.SaveChangesAsync();
                }

                var notifyUserAvailableProducts = (from p in userToNotifyReference
                                                   select new MovieOutOfStock
                                                   {
                                                       UserAttemptToCartOutOfStockId = p,
                                                       DateCreation=now,
                                                       MovieId = gt.MovieId,
                                                       IsOutOfStock = false
                                                   }).ToList();

                await _context.MovieOutOfStocks.AddRangeAsync(notifyUserAvailableProducts);
                await _context.SaveChangesAsync();

                var userToNotifyStockFalse = await _context.MovieOutOfStocks
                   .Include(w => w.UserAttemptToCartOutOfStock)
                   .Include(w => w.Movie).ThenInclude(w => w.Vendor)
                   .Where(w => w.MovieId == gt.MovieId && w.IsOutOfStock == false)
                   .ToListAsync();
                var movieOutOfStockNotification = (from p in userToNotifyStockFalse
                                                   select new MovieOutOfStockNotification
                                                   {
                                                       ReceivingUserId = p.UserAttemptToCartOutOfStockId,
                                                       MovieOutOfStockId = p.Id,
                                                       Creation = now,
                                                       Header = $"Good Day {p.UserAttemptToCartOutOfStock.UserName}! The product you`ve selected is now available.",
                                                       MessageBody = $"The {p.Movie.Name} from {p.Movie.Vendor.UserName} is Now AVAILABLE. Grab it now ",
                                                       ProductImage = p.Movie.ImageURL
                                                   }).ToList();

                await _context.MovieOutOfStockNotifications.AddRangeAsync(movieOutOfStockNotification);
                await _context.SaveChangesAsync(); 

                return Ok($"Successfully added stock count by: {gt.StockNumber}");
            }
            return BadRequest("Updating Stock went error");
        }


        [HttpGet("Search")]
        [AllowAnonymous]
        public async Task<ActionResult<SearchingProductView>> SearchingProduct([FromQuery] SearchProductDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);

            var productQuery = _context.Movies.AsQueryable();

            if(string.IsNullOrEmpty(dto.keyword) == false)
            {
                productQuery = productQuery.Where(w => w.Name.ToLower().Contains(dto.keyword.Trim().ToLower()));
            }

            if (dto.highestToLowest)
            {
                productQuery = productQuery.OrderByDescending(w => w.Price);
            }
            if (dto.lowestToHighest)
            {
                productQuery = productQuery.OrderBy(w => w.Price);
            }

            var productInformation = await (from m in productQuery
                                            select new ProductInformationDTO()
                                            {
                                                MovieId = m.Id,
                                                Price = m.Price,
                                                ProductAddress = m.Vendor.Address,
                                                ProductImage = m.ImageURL,
                                                ProductName = m.Name,
                                                TotalSold = m.Sold,
                                            }).Paginate(dto.PaginationDTO).AsNoTracking().ToListAsync();

            return new SearchingProductView
            {
                ProductInformation = productInformation,
                SearchQuery = dto.keyword,
                
            };
        }
    }
}
