using Application.Helper;
using Application.Repository.IService;
using Application.ViewModel.ReturnRequest;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service
{
    public class ReturnProductRepository : IReturnProduct
    {
        private readonly ApplicationDbContext _context;
        const int VALID_DAYS = 2;
        private readonly IFileStorageService fileStorageService;
        private string container = "returnProducts";
        public ReturnProductRepository(ApplicationDbContext context, IFileStorageService fileStorageService)
        {
            _context = context;
            this.fileStorageService = fileStorageService;
        }

        public async Task ReturnRequestProduct(ReturnRequestCreationDTO dto, string userId, string userName)
        {
            var vendorId = _context.PlaceOrderItems
                .FirstOrDefault(w => w.Id == dto.PlaceOrderItemsId).VendorId;

            // Check if you placed an order.
            var checkIfOrderExists = await _context.PlaceOrderItems
                  .Where(w => w.UserPlaceOrderId == userId)
                  .AnyAsync(w => w.Id == dto.PlaceOrderItemsId);

            var placeOrder = await _context.PlaceOrderItems
                .Include(w => w.Vendor)
                .Where(w => w.UserPlaceOrderId == userId)
                .FirstOrDefaultAsync(w => w.Id == dto.PlaceOrderItemsId);

            var transaction = await _context.ProductTransactions
                 .Where(w => w.ConsumerId == userId)
                 .Include(w => w.Vendor)
                 .Include(w => w.PlaceOrderItems).ThenInclude(w => w.Movie)
                 .FirstOrDefaultAsync(w => w.PlaceOrderItemsId == placeOrder.Id);
 
            // Requesting product needs to have less day 2 days after the order received.
            var now = DateTime.Now;

            var validRequestTime = transaction.DateReceived - now;
            
            if (checkIfOrderExists && validRequestTime.Days <= VALID_DAYS)
            {

                var request = await ConsumerRequestForReturn(dto, userId, vendorId);

                if (dto.ProductImages != null)
                {
                    await BuildListImages(dto.ProductImages, request.Id, userId);
                }
                // notification for the vendor.
                await AddNotificationToVendor(userName, transaction, now, request);

                /* return Ok($"Wait for {placeOrder.Vendor.UserName} to verify your request.");*/
            }
        }

        private async Task<ReturnProduct> ConsumerRequestForReturn(ReturnRequestCreationDTO dto, string userId, string vendorId)
        {
            var request = new ReturnProduct()
            {
                ConsumerId = userId,
                DateRequest = DateTime.Now,
                Description = dto.Description,
                PlaceOrderItemsId = dto.PlaceOrderItemsId,
                ReturnReasonsId = dto.ReturnReasonsId,
                VendorId = vendorId,
            };
            await _context.ReturnProducts.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        private async Task AddNotificationToVendor(string userName, ProductTransaction transaction, DateTime now, ReturnProduct request)
        {
            var returnNotif = new ReturnProductNotification()
            {
                Creation = now,
                Header = $"Good Day Seller! {userName} has filed a return product. Assist it immediately",
                MessageBody = $"Hi {transaction.Vendor.UserName}, The {transaction.PlaceOrderItems.Movie.Name} product seems wanted to return by {userName}",
                ProductImage = transaction.PlaceOrderItems.Movie.ImageURL,
                ReceivingUserId = transaction.VendorId,
                ReturnProductId = request.Id
            };

            await _context.ReturnProductNotifications.AddAsync(returnNotif);
            await _context.SaveChangesAsync();
        }

        private async Task<List<ReturnProductImage>> BuildListImages(List<IFormFile> productImages, int requestId, string userId)
        {
            var res = new List<ReturnProductImage>();

            if (productImages != null)
            {
                foreach (var item in productImages)
                {
                    res.Add(new ReturnProductImage
                    {
                        ConsumerId = userId,
                        ReturnProductId = requestId,
                        ProductImage = await fileStorageService.SaveFile(container, item)
                    });
                }
                await _context.ReturnProductImages.AddRangeAsync(res);
                await _context.SaveChangesAsync();
            }
            return res;
        }

    }
}
