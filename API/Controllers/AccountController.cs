using Application.Helper;
using Application.Repository.IService;
using Application.Repository.Service;
using Application.ViewModel.Account;
using Application.ViewModel.Movie;
using Application.ViewModel.PlaceOrder;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private string container = "users";
        private readonly IAccount _account;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IMapper mapper, RoleManager<IdentityRole> roleManager,
            IFileStorageService fileStorageService,
            IAccount account)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
            this.mapper = mapper;
            _roleManager = roleManager;
            this.fileStorageService = fileStorageService;
            _account = account;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(
             [FromBody] LoginDTO userCredentials)
        {
            var result = await signInManager.PasswordSignInAsync(userCredentials.Email,
                userCredentials.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildTokendd(userCredentials);
            }
            else
            {
                return BadRequest("Incorrect Login");
            }
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<AuthenticationResponse>> Register(
          [FromForm] RegisterDTO userCredentials)
        {
            if (_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("User"));
                await _roleManager.CreateAsync(new IdentityRole("Vendor"));
                await _roleManager.CreateAsync(new IdentityRole("Rider"));

            }
            var user = new ApplicationUser
            {
                UserName = userCredentials.Email,
                Email = userCredentials.Email,
                Role = userCredentials.RoleName,
                FirstName = userCredentials.FirstName,
                LastName = userCredentials.LastName,
                PhoneNumber = "09151516357",
                Address = "49 Pechay St Napico Pasig City"
            };
            if (userCredentials.ImageUrl != null)
            {
                user.ImageUrl = await fileStorageService.SaveFile(container, userCredentials.ImageUrl);
            }

            var result = await userManager.CreateAsync(user, userCredentials.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, userCredentials.RoleName);

                return await BuildTokendd(userCredentials);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        private async Task<AuthenticationResponse> BuildTokendd(RegisterDTO userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email),

            };

            var user = await userManager.FindByNameAsync(userCredentials.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                RoleName = userCredentials.RoleName
            };
        }

        private async Task<AuthenticationResponse> BuildTokendd(LoginDTO userCredentials)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", userCredentials.Email),
            };

            var user = await userManager.FindByNameAsync(userCredentials.Email);

            var claimsDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: creds);

            return new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                RoleName = user.Role
            };
        }

        [HttpGet("MyAccount")]
        public async Task<ActionResult<MyAccountView>> MyAccount()
        {
            var user = await userManager.GetUserAsync(User);

            return await _account.MyAccount(user.Id);
        }
        [HttpGet("ToPayView")]
        public async Task<ActionResult<List<ToPayView>>> ToPayView()
        {
            var user = await userManager.GetUserAsync(User);

            return await _account.ToPayView(user.Id);
        }
        [HttpPut("EditMyAccount")]
        public async Task<ActionResult> EditMyAccount([FromForm] ModifyMyAccount accountEditDTO)
        {
            var user = await userManager.GetUserAsync(User);

            await _account.EditMyAccount(accountEditDTO, user.Id);

            return Ok();
        }
        [HttpGet("MyPurchaseHistory")]
        public async Task<ActionResult<List<MyPurchaseProduct>>> PurchaseHistory()
        {
            var user = await userManager.GetUserAsync(User);

            return await _account.PurchaseHistory(user.Id);
        }

        [HttpGet("VendorShop/{vendorId}")]
        [AllowAnonymous]
        public async Task<ActionResult<VendorShopView>> VendorShop(string vendorId)
        {
            var user = await userManager.GetUserAsync(User);
            
            return await _account.VendorShop(user.Id);
        }

        [HttpGet("VendorListOfProducts")]
        [AllowAnonymous]
        public async Task<ActionResult<List<MyProductInformation>>> VendorListOfProducts([FromQuery] MyShopPaginationFilter dto)
        {
            var user = await userManager.GetUserAsync(User);

            var movieUsers = await context.MovieUserViews
                .Where(w => w.Movie.VendorId == user.Id)
                .ToListAsync();
            var movieQuery = context.Movies.AsQueryable();

            if (string.IsNullOrEmpty(dto.MovieName) == false)
            {
                movieQuery = movieQuery.Where(w => dto.MovieName.Contains(w.Name));
            }

            if (string.IsNullOrEmpty(dto.MovieCategory) == false)
            {
                movieQuery = movieQuery.Where(w => dto.MovieCategory.Contains(w.ProductCategory));
            }

            if (dto.TopSale == true)
            {
                movieQuery = movieQuery.OrderByDescending(w => w.Sold);
            }
            var myProductInformation = await (from m in context.Movies
                                              where m.VendorId == user.Id

                                              select new MyProductInformation
                                              {
                                                  MovieId = m.Id,
                                                  MovieImage = m.ImageURL,
                                                  MovieName = m.Name,
                                                  Price = m.Price,
                                                  Sales = m.Sold,
                                                  AvailableProduct = m.StockCount,
                                                  ProductCategory = m.ProductCategory,
                                                  UserView = (from t in m.MovieUserViews
                                                              select new UserWhoViewed
                                                              {
                                                                  CustomerId = t.UserWhoViewId,
                                                                  CustomerName = t.UserWhoView.UserName,
                                                                  DateViewed = t.ViewDate
                                                              }).ToList()
                                              }).Paginate(dto.PaginationDTO).AsNoTracking().ToListAsync();

            return myProductInformation;
        }

        [HttpGet("VendorProductPurchaseHistory")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult
            <VendorProductSalesView>> VendorProductPurchaseHistory([FromQuery] VendorPurchaseHistoryPaginate dto)
        {
            var user = await userManager.GetUserAsync(User);
            IEnumerable<GroupingProductPurchaseHistory> groupingPurchaseHistory = null;

            var placeOrderQuery = context.PlaceOrderItems
                .Include(w => w.Movie)
                .AsQueryable();

            if (string.IsNullOrEmpty(dto.MovieName) == false)
                placeOrderQuery = placeOrderQuery.Where(w => dto.MovieName.Contains(w.Movie.Name));

            /*            if (string.IsNullOrEmpty(dto.DatePurchased) == false)
                        {
                            var asd = placeOrderQuery.Select(w => w.PlacedOrderCreation.ToString("MM")).ToList();
                            placeOrderQuery = placeOrderQuery.Where(w =>  asd.Contains(dto.DatePurchased));

                        }*/
            if (dto.DatePurchased != new DateTime())
                placeOrderQuery = placeOrderQuery.Where(w => w.PlacedOrderCreation == dto.DatePurchased);


            var purchaseHistory = await (from p in placeOrderQuery
                                         join pt in context.ProductTransactions on p.Id equals pt.PlaceOrderItemsId
                                         where p.VendorId == user.Id
                                         where pt.IsReturned == false
                                         select new ProductPurchaseHistory()
                                         {
                                             Amount = p.Amount,
                                             Price = p.Price,
                                             CustomerId = pt.ConsumerId,
                                             CustomerName = pt.Consumer.FirstName + " " + pt.Consumer.LastName,
                                             MovieId = p.MovieId,
                                             MovieName = p.Movie.Name,
                                             DatePurchased = p.PlacedOrderCreation,
                                             RiderId = pt.RiderId,
                                             RiderName = pt.Rider.FirstName + " " + pt.Rider.LastName,
                                             ProductRate = pt.Rate,
                                             AvailableProduct = p.Movie.StockCount
                                         }).Paginate(dto.PaginationDTO).AsNoTracking().ToListAsync();

            groupingPurchaseHistory = GroupHistory(purchaseHistory);

            // filter by date
            // paginate
            return new VendorProductSalesView
            {
                ProductPurchaseHistories = purchaseHistory,
                TotalSales = purchaseHistory.Select(w => w.Amount * w.Price).Sum()
            };
        }
        [HttpGet("ViewBuyerRating/{ratingProductid}")]
        public async Task<ActionResult<ProductRateReview>> ViewBuyerRating(int ratingProductid)
        {
            var user = await userManager.GetUserAsync(User);

            // Make sure the vendor could only his customer that were
            // being registered in the placeOrderItems

            var rateProduct = await context.RateProducts
                .Include(w => w.Movie)
                .FirstOrDefaultAsync(w => w.Id == ratingProductid);

            if (rateProduct == null) return BadRequest("The rate product you`re looknig for is unknown.");

            var productTrans = await context.ProductTransactions
                .Where(w => w.ConsumerId == rateProduct.UserWhoHadRateId)
                .AnyAsync(w => w.RateId == rateProduct.Id);

            var userReviews = new ProductRateReview();

            if (productTrans == true)
            {
                userReviews = await (from r in context.RateProducts
                                     where r.Movie.VendorId == user.Id
                                     where r.Id == ratingProductid

                                     select new ProductRateReview()
                                     {
                                         DateCreated = r.RateCreation.ToString("dd/M/yyyy HH:mm tt"),
                                         RateCount = r.RateCount,
                                         Username = r.UserWhoHadRate.FirstName + " " + r.UserWhoHadRate.LastName,
                                         ImageUrl = r.ImageUrl,
                                         Message = r.Message,
                                         MovieId = r.MovieId,
                                     }).AsNoTracking().FirstOrDefaultAsync();
            }
            else
            {
                return BadRequest("The rate user you`re looknig for is unknown.");
            }

            return userReviews;
        }
        private IEnumerable<GroupingProductPurchaseHistory> GroupHistory(List<ProductPurchaseHistory>
            purchaseHistory)
        {
            var item = (from p in purchaseHistory
                        group p by p.DatePurchased into g
                        select new GroupingProductPurchaseHistory()
                        {
                            DatePurchased = g.Select(w => w.DatePurchased).FirstOrDefault(),
                            Items = g
                        });
            return item;
        }

        [HttpGet("ProductNotification")]
        public async Task<ActionResult<List<ProductNotificationView>>> ProductNotification()
        {
            var user = await userManager.GetUserAsync(User);
            var productNotification = new List<ProductNotificationView>();
            var voucherNotification = new List<ProductNotificationView>();
            var messageNotification = new List<ProductNotificationView>();
            var returnProductNotification = new List<ProductNotificationView>();
            var voucherExpirationNotification = new List<ProductNotificationView>();
            var productOutOfStockNotification = new List<ProductNotificationView>();

            if (user.Role == "User")
            {
                productNotification = await (from n in context.ProductTransactionNotif
                                             where n.ReceivingUserId == user.Id
                                             select new ProductNotificationView
                                             {
                                                 Creation = n.Creation.ToString("dd/M/yyyy HH:mm tt"),
                                                 Header = n.Header,
                                                 Message = n.MessageBody,
                                                 Picture = n.ProductImage,
                                                 Path = $"/api/PlaceOrder/user/purchase/order/{n.ProductTransaction.PlaceOrderItemsId}",

                                             }).AsNoTracking().ToListAsync();

                // It would redirect to the product which has a discounted price setted by vendor
                voucherNotification = await (from n in context.VoucherNotifications
                                             where n.ReceivingUserId == user.Id
                                             select new ProductNotificationView
                                             {
                                                 Creation = n.Creation.ToString("dd/M/yyyy HH:mm tt"),
                                                 Header = n.Header,
                                                 Message = n.MessageBody,
                                                 Picture = n.ProductImage,
                                                 Path = $"/api/Movie/SingleMovie/{n.Voucher.MovieId}",
                                             }).AsNoTracking().ToListAsync();

                // Users has a inbox message to the vendor
                messageNotification = await (from n in context.MessageNotifications
                                             where n.ReceivingUserId == user.Id
                                             select new ProductNotificationView
                                             {
                                                 Creation = n.Creation.ToString("dd/M/yyyy HH:mm tt"),
                                                 Header = n.Header,
                                                 Message = n.MessageBody,
                                                 Picture = n.UserWhomMake.ImageUrl,
                                                 Path = $"/api/MessageTable/GetChatWithSomeone/{n.MessageTableId}",

                                             }).AsNoTracking().ToListAsync();

                productOutOfStockNotification = await (from n in context.MovieOutOfStockNotifications
                                                       where n.ReceivingUserId == user.Id
                                                       where n.MovieOutOfStock.IsClicked == false
                                                       select new ProductNotificationView
                                                       {
                                                           Creation = n.Creation.ToString("dd/M/yyyy HH:mm tt"),
                                                           Header = n.Header,
                                                           Message = n.MessageBody,
                                                           Picture = n.MovieOutOfStock.Movie.ImageURL,
                                                          /* MovieOutOfStockId = n.MovieOutOfStockId*/
                                                          Path = $"/api/Movie/SingleMovie/{n.MovieOutOfStock.MovieId}",
                                                       }).AsNoTracking().ToListAsync();

                productNotification = productNotification.Concat(voucherNotification)
                    .Concat(messageNotification).Concat(productOutOfStockNotification).ToList();

            }
            else if (user.Role == "Vendor")
            {
                productNotification = await (from n in context.ProductTransactionNotif
                                             where n.ReceivingUserId == user.Id
                                             select new ProductNotificationView
                                             {
                                                 Creation = n.Creation.ToString("dd/M/yyyy HH:mm tt"),
                                                 Header = n.Header,
                                                 Message = n.MessageBody,
                                                 Picture = n.ProductImage
                                             }).AsNoTracking().ToListAsync();

                returnProductNotification = await (from n in context.ReturnProductNotifications
                                                   where n.ReceivingUserId == user.Id
                                                   select new ProductNotificationView
                                                   {
                                                       Creation = n.Creation.ToString("dd/M/yyyy HH:mm tt"),
                                                       Header = n.Header,
                                                       Message = n.MessageBody,
                                                       Picture = n.ProductImage,
                                                       Path = $"/api/ReturnProduct/ReturnProductView    /{n.ReturnProductId}",
                                                   }).AsNoTracking().ToListAsync();

                messageNotification = await (from n in context.MessageNotifications
                                             where n.ReceivingUserId == user.Id
                                             select new ProductNotificationView
                                             {
                                                 Creation = n.Creation.ToString("dd/M/yyyy HH:mm tt"),
                                                 Header = n.Header,
                                                 Message = n.MessageBody,
                                                 Picture = n.UserWhomMake.ImageUrl,
                                                 Path = $"/api/MessageTable/GetChatWithSomeone/{n.MessageTableId}",

                                             }).AsNoTracking().ToListAsync();

                // Todo list of vendor Voucher
                voucherExpirationNotification = await (from n in context.VoucherNotifications
                                                       where n.ReceivingUserId == user.Id
                                                       select new ProductNotificationView
                                                       {
                                                           Creation = n.Creation.ToString("dd/M/yyyy HH:mm tt"),
                                                           Header = n.Header,
                                                           Message = n.MessageBody,
                                                           Picture = n.Voucher.Movie.ImageURL,
                                                           Path = $"/api/Voucher/AllVoucher/{n.Voucher}",
                                                       }).AsNoTracking().ToListAsync();

                productNotification = productNotification.Concat(returnProductNotification)
                    .Concat(messageNotification).Concat(voucherExpirationNotification).ToList();


            }


            return productNotification.ToList();
        }
        [HttpGet("UserClickedMovieOutOfStock/{movieOutOfStockId}")]
        
        public async Task<ActionResult<SellerInventoryDTO>> UserClickedMovieOutOfStock(int movieOutOfStockId)
        {
            var user = await userManager.GetUserAsync(User);

            var movieOutOfStock = await context.MovieOutOfStocks
                .Where(w=> w.UserAttemptToCartOutOfStockId == user.Id)
                .FirstOrDefaultAsync(w => w.Id == movieOutOfStockId);

            var now = DateTime.Now;

            // movie out o f stock = click true
            // movie available notif - delete.

            if (movieOutOfStock != null && movieOutOfStock.IsOutOfStock == true)
            {
                var dateClick = movieOutOfStock?.DateCreation;
                // once you clicked, it will redirect to product itself
                movieOutOfStock.IsClicked = true;
                 
                /*context.MovieOutOfStocks.Remove(movieOutOfStock);*/
                await context.SaveChangesAsync();
        
                return Ok("Movie out of stock clicked.");

            }else if (movieOutOfStock != null && movieOutOfStock.IsOutOfStock == false)
            {


                context.MovieOutOfStocks.Remove(movieOutOfStock);
                await context.SaveChangesAsync();

                return Ok("Movie out of stock notification is now deleted.");

            }


            return BadRequest("Clicking movie out of stock notification went error.");
        }
            [HttpGet("SellerInventory")]
        [Authorize(Roles ="Vendor")]
        public async Task<ActionResult<SellerInventoryDTO>> SellerInventory()
        {
            var user = await userManager.GetUserAsync(User);
            var now = DateTime.Now;

            int firstToSevenDays = 7;
            int firstToFourteenDays = 14;
            int firstToTwentyOne = 21;
            int firstToTwentyEight = 30;


            var movieNotifMovieId = context.MovieNotifications.Select(w => w.MovieId).ToList();

            // Tracked the date purchased of the product. It must be a REAL sales, No Return
            // Tracked the First 7th, 14th, 21th, 28th day if the month.

            int firstQuarter = CountReturnedProducts(firstToSevenDays);
            int secondQuarter = CountReturnedProducts(firstToFourteenDays);
            int thirdQuarter = CountReturnedProducts(firstToTwentyEight);
            int fourthQuarter = CountReturnedProducts(firstToTwentyEight);

            var firstSevenProducts = await (from t in context.ProductTransactions
                                            where t.VendorApproved == true && t.VendorId == user.Id
                                            where t.PlaceOrderItems.PlacedOrderCreation.Day <= firstToSevenDays
                                            select new SalesOnFirstSevenDay
                                            {
                                                MovieId = t.PlaceOrderItems.MovieId,
                                                MovieName = t.PlaceOrderItems.Movie.Name,
                                                MoviePicture = t.PlaceOrderItems.Movie.ImageURL,
                                                Rating = t.Rate,
                                                // How check if the product had a discount of the product
                                                Discount = t.PlaceOrderItems.TotalDiscount,
                                                Sales = t.PlaceOrderItems.Price,
                                                NumberOfReturned = firstQuarter
                                            }).AsNoTracking().ToListAsync();

            var firstFourteenProducts = await (from t in context.ProductTransactions
                                               where t.VendorApproved == true && t.VendorId == user.Id
                                               where t.PlaceOrderItems.PlacedOrderCreation.Day >= firstToSevenDays && t.PlaceOrderItems.PlacedOrderCreation.Day <= firstToFourteenDays
                                               select new SalesOnFirstSevenDay
                                               {
                                                   MovieId = t.PlaceOrderItems.MovieId,
                                                   MovieName = t.PlaceOrderItems.Movie.Name,
                                                   MoviePicture = t.PlaceOrderItems.Movie.ImageURL,
                                                   Rating = t.Rate,
                                                   Discount = t.PlaceOrderItems.TotalDiscount,
                                                   Sales = t.PlaceOrderItems.Price,
                                                   NumberOfReturned = secondQuarter
                                               }).AsNoTracking().ToListAsync();

            var firstTwentyOneProducts = await (from t in context.ProductTransactions
                                                where t.VendorApproved == true && t.VendorId == user.Id
                                                where t.PlaceOrderItems.PlacedOrderCreation.Day >= firstToFourteenDays && t.PlaceOrderItems.PlacedOrderCreation.Day <= firstToTwentyOne
                                                select new SalesOnFirstSevenDay
                                                {
                                                    MovieId = t.PlaceOrderItems.MovieId,
                                                    MovieName = t.PlaceOrderItems.Movie.Name,
                                                    MoviePicture = t.PlaceOrderItems.Movie.ImageURL,
                                                    Rating = t.Rate,
                                                    Discount = t.PlaceOrderItems.TotalDiscount,
                                                    Sales = t.PlaceOrderItems.Price,
                                                    NumberOfReturned = thirdQuarter
                                                }).AsNoTracking().ToListAsync();

            var firstTwentyEightProducts = await (from t in context.ProductTransactions
                                                  where t.VendorApproved == true && t.VendorId == user.Id
                                                  where t.PlaceOrderItems.PlacedOrderCreation.Day >= firstToTwentyOne && t.PlaceOrderItems.PlacedOrderCreation.Day <= 30
                                                  select new SalesOnFirstSevenDay
                                                  {
                                                      MovieId = t.PlaceOrderItems.MovieId,
                                                      MovieName = t.PlaceOrderItems.Movie.Name,
                                                      MoviePicture = t.PlaceOrderItems.Movie.ImageURL,
                                                      Rating = t.Rate,
                                                      Discount = t.PlaceOrderItems.TotalDiscount,
                                                      Sales = t.PlaceOrderItems.Price,
                                                      NumberOfReturned = fourthQuarter
                                                  }).AsNoTracking().ToListAsync();


            var movieRunningOutOfStock = await context.Movies
                .Include(w => w.Vendor)
                .Where(w => w.VendorId == user.Id)
                .Where(w => w.StockCount <= 48)
                .Where(w => movieNotifMovieId.Contains(w.Id) == false)
                .ToListAsync();

            var movieOutOfStockNotif = (from m in movieRunningOutOfStock
                                        select new MovieNotification
                                        {
                                            Header = $"Hi {m.Vendor.UserName}, It seems one of your product is running out of stock",
                                            MessageBody = $"The {m.Name} has {m.StockCount} left. Restock immediately to avoid unwanted loss of sales",
                                            Creation = now,
                                            MovieId = m.Id,
                                            ProductImage = m.ImageURL,
                                            ReceivingUserId = user.Id
                                        }).ToList();
 
            await context.MovieNotifications.AddRangeAsync(movieOutOfStockNotif);
            await context.SaveChangesAsync();

            return new SellerInventoryDTO
            {
                SalesOnFirstQuarter = firstSevenProducts,
                SalesOnSecondQuarter = firstFourteenProducts,
                SalesOnThirdQuarter = firstTwentyOneProducts,
                SalesOnFourthQuarter = firstTwentyEightProducts,
            };
        }

        private int CountReturnedProducts(int firstToSevenDays)
        {
            var dd = context.ProductTransactions
                             .Where(w => w.PlaceOrderItems.PlacedOrderCreation.Day <= firstToSevenDays)
                             .Where(w => w.IsReturned == true)
                             .Count();
            return dd;
        }

        public static List<DateTime> GetDates(int year, int month, int numberOfDays)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                             .Select(day => new DateTime(year, month, day))
                             .Take(numberOfDays)
                             .ToList();
        }
    }
}
