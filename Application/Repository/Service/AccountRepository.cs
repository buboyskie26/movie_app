using Application.Repository.IService;
using Application.ViewModel.Account;
using Domain;
using Microsoft.AspNetCore.Identity;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.ViewModel.PlaceOrder;
using AutoMapper;
using Application.Helper;
using Application.ViewModel.Movie;
using Application.BaseRepository;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Repository.Service
{
    public class AccountRepository : IAccount
    {
        private readonly ApplicationDbContext _context;
        private readonly IBaseRepository<ApplicationUser> _base;

        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private string container = "users";
        public AccountRepository(ApplicationDbContext context, IMapper mapper, IFileStorageService fileStorageService, 
            IBaseRepository<ApplicationUser> @base)
        {
            _context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
            _base = @base;
        }

        public async  Task EditMyAccount(ModifyMyAccount accountEditDTO, string userId)
        {
            var myAccount = await _context.ApplicationUsers
               .Where(w => w.Id == userId)
               .FirstOrDefaultAsync(w => w.Id == accountEditDTO.Id);

            myAccount = mapper.Map(accountEditDTO, myAccount);

            if (accountEditDTO.ImageUrl != null)
            {
                myAccount.ImageUrl = await fileStorageService.EditFile(container,
                    accountEditDTO.ImageUrl, myAccount.ImageUrl);
            }
            // Automapper will handle the update.
            /*context.ApplicationUsers.Update(myAccount);*/
            await _context.SaveChangesAsync();
        }

        public async Task<MyAccountView> MyAccount(string userId)
        {
            var obj = await (from u in _context.ApplicationUsers
                             where u.Id == userId

                             select new MyAccountView
                             {
                                 UserId = userId,
                                 Address = u.Address,
                                 FirstName = u.FirstName,
                                 LastName = u.LastName,
                                 ImageProfile = u.ImageUrl,
                                 PhoneNumber = u.PhoneNumber
                             }).FirstOrDefaultAsync();

            return obj;
        }

        public async Task<List<MyPurchaseProduct>> PurchaseHistory(string userId)
        {
            var myOrder = await(from s in _context.PlaceOrderItems
                                where s.UserPlaceOrderId == userId
                                select new MyPurchaseProduct
                                {
                                    Amount = s.Amount,
                                    Price = s.Price,
                                    MovieId = s.MovieId,
                                    MovieName = s.Movie.Name,
                                    DatePurchase = s.PlacedOrderCreation,
                                    PlaceOrderItemsId = s.Id
                                }).AsNoTracking().ToListAsync();
            return myOrder;
        }

        public async Task<List<ToPayView>> ToPayView(string userId)
        {
            /*var productTransaction = _context.ProductTransactions
                .Include(w => w.PlaceOrderItems)
                .FirstOrDefault(w => w.ConsumerId == userId);

            var placeOrderCreation = productTransaction.PlaceOrderItems.PlacedOrderCreation.ToLocalTime().AddDays(5).Day;*/

            var transaction = await (from pt in _context.ProductTransactions

                                     where pt.ConsumerId == userId
                                     select new ToPayView()
                                     {
                                         Amount = pt.PlaceOrderItems.Amount,
                                         PlaceOrderId = pt.PlaceOrderItemsId,
                                         MovieId = pt.PlaceOrderItems.MovieId,
                                         MovieName = pt.PlaceOrderItems.Movie.Name,
                                         Price = pt.PlaceOrderItems.Movie.Price,
                                         TransactionId = pt.Id,
                                         RiderId = pt.RiderId,
                                         RiderName = pt.Rider.FirstName + " " + pt.Rider.LastName,
                                         // Bug what if the day starts in 30 and add 3 days, it did not add into 33
                                         ExpectedDayToDeliver = pt.PlaceOrderItems.PlacedOrderCreation.ToLocalTime().AddDays(3).Day - DateTime.Now.Day
                                     }).AsNoTracking().ToListAsync();

            return transaction;
        }

        public async Task<VendorShopView> VendorShop(string vendorId)
        {
            var check = await _base.CheckExists(vendorId);
            if (check != null)
            {
                /*return HttpError("Theres a no vendor exists");*/
                /*throw new Badre("ert");*/
                /* throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                 {
                     Content = new StringContent("Vendor Error"),
                     ReasonPhrase = "Critical Exception"
                 });*/
                /*throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {

                });*/

                /*var message = string.Format("Product with id = {0} not found", id);
                HttpError err = new HttpError(message);
                return Request.CreateResponse(HttpStatusCode.NotFound, err);*/
                 
            }

            var totalProducts = await _context.Movies
                .Where(w => w.VendorId == vendorId)
                .Select(w => w.Id).CountAsync();

            var totalRating = await _context.RateProducts
                .Include(w => w.Movie)
                .Where(w => w.Movie.VendorId == vendorId)
                .Select(w => w.RateCount).CountAsync();

            var getCategories = await _context.Movies
                 .Where(w => w.VendorId == vendorId)
                 .Select(w => w.ProductCategory).OrderBy(w => w)
                 .Distinct().ToListAsync();

            var topProducts = await _context.Movies
             .Where(w => w.VendorId == vendorId)
             .Distinct()
             .Select(m => new ProductFilteredByCategory
             {
                 MovieId = m.Id,
                 Price = m.Price,
                 ProductAddress = m.Vendor.Address,
                 ProductImage = m.ImageURL,
                 ProductName = m.Name,
                 TotalSold = m.Sold,
                 CategoryTopic = m.ProductCategory
             })
             .Take(5)
             .OrderByDescending(w => w.TotalSold)
             .ToListAsync();

            var vendorFirstPhase = await (from s in _context.Movies

                                          where s.VendorId == vendorId
                                          select new VendorSummary
                                          {
                                              MovieId = s.Id,
                                              TotalProducts = totalProducts,
                                              VendorName = s.Vendor.FirstName + " " + s.Vendor.LastName,
                                              TotalRating = totalRating,
                                              ProductCategory = getCategories,

                                          }).FirstOrDefaultAsync();

            var productInformation = await (from m in _context.Movies
                                            where getCategories.Contains(m.ProductCategory)
                                            select new ProductFilteredByCategory()
                                            {
                                                MovieId = m.Id,
                                                Price = m.Price,
                                                ProductAddress = m.Vendor.Address,
                                                ProductImage = m.ImageURL,
                                                ProductName = m.Name,
                                                TotalSold = m.Sold,
                                                CategoryTopic = m.ProductCategory
                                            }).AsNoTracking().ToListAsync();

            var groupProductInformation = (from it in productInformation
                                           group it by it.CategoryTopic into m
                                           select new GroupProductFilteredByCategory
                                           {
                                               CategoryTopic = m.Key,
                                               ProductFilteredByCategories = m.ToList()
                                           });

            /*groupItem = GroupProductInformation( productInformation);*/
            return new VendorShopView
            {
                VendorSummary = vendorFirstPhase,
                TopProducts = topProducts,
                GroupProductFilteredByCategories = groupProductInformation,
            };
        }
    }
}
