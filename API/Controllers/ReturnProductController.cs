using Application.Helper;
using Application.Repository.IService;
using Application.ViewModel.ReturnRequest;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReturnProductController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService fileStorageService;
        private readonly IReturnProduct _returnProduct;
        private string container = "returnProducts";
        const int VALID_DAYS = 2;

        public ReturnProductController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IFileStorageService fileStorageService, IReturnProduct returnProduct)
        {
            _context = context;
            _userManager = userManager;
            this.fileStorageService = fileStorageService;
            _returnProduct = returnProduct;
        }



        // Assumed that the customer already received the product.


        [HttpPost("ProductReturnRequest")]
        // Consumer route
        public async Task<ActionResult> ProductReturnRequest([FromForm] ReturnRequestCreationDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);

            /*var vendorId = _context.PlaceOrderItems
                .FirstOrDefault(w => w.Id == dto.PlaceOrderItemsId).VendorId;

            // Check if you placed an order.
            var checkIfOrderExists = await _context.PlaceOrderItems
                  .Where(w => w.UserPlaceOrderId == user.Id)
                  .AnyAsync(w => w.Id == dto.PlaceOrderItemsId);

            var placeOrder = await _context.PlaceOrderItems
                .Include(w=> w.Vendor)
                .Where(w=> w.UserPlaceOrderId == user.Id)
                .FirstOrDefaultAsync(w => w.Id == dto.PlaceOrderItemsId);

            var transaction = await _context.ProductTransactions
                 .Where(w => w.ConsumerId == user.Id)
                 .Include(w=> w.Vendor)
                 .Include(w=> w.PlaceOrderItems).ThenInclude(w=> w.Movie)
                 .FirstOrDefaultAsync(w => w.PlaceOrderItemsId == placeOrder.Id);

            // Todo: 

            // Requesting product needs to have less day 2 days after the order received.
            var now = DateTime.Now;

            var validRequestTime = transaction.DateReceived - now;

            if (checkIfOrderExists && validRequestTime.Days <= VALID_DAYS)
            {
                var request = new ReturnProduct()
                {
                    ConsumerId = user.Id,
                    DateRequest = DateTime.Now,
                    Description = dto.Description,
                    PlaceOrderItemsId = dto.PlaceOrderItemsId,
                    ReturnReasonsId = dto.ReturnReasonsId,
                    VendorId = vendorId,
                };

                *//*if (dto.ProductImage != null)
                    request.ProductImage = await fileStorageService.SaveFile(container, dto.ProductImage);*//*
 
                await _context.ReturnProducts.AddAsync(request);
                await _context.SaveChangesAsync();

                if (dto.ProductImages != null)
                {
                      await BuildListImages(dto.ProductImages, request.Id);
                }
                // notification for the vendor.
                var returnNotif = new ReturnProductNotification()
                {
                    Creation = now,
                    Header = $"Good Day Seller! {user.UserName} has filed a return product. Assist it immediately",
                    MessageBody = $"Hi {transaction.Vendor.UserName}, The {transaction.PlaceOrderItems.Movie.Name} product seems wanted to return by {user.UserName}",
                    ProductImage = transaction.PlaceOrderItems.Movie.ImageURL,
                    ReceivingUserId = transaction.VendorId,
                    ReturnProductId = request.Id
                };

                await _context.ReturnProductNotifications.AddAsync(returnNotif);
                await _context.SaveChangesAsync();

                return Ok($"Wait for {placeOrder.Vendor.UserName} to verify your request.");
            }*/
            await _returnProduct.ReturnRequestProduct(dto, user.Id, user.UserName);

            return BadRequest("Requesting Error");
        }

        private async Task< List<ReturnProductImage>> BuildListImages(List<IFormFile> productImages, int requestId)
        {
            var res = new List<ReturnProductImage>();
            var user = await _userManager.GetUserAsync(User);

            if (productImages != null)
            {
                foreach (var item in productImages)
                {
                    res.Add(new ReturnProductImage
                    {
                        ConsumerId = user.Id,
                        ReturnProductId = requestId,
                        ProductImage = await fileStorageService.SaveFile(container, item)
                    });
                }
                await _context.ReturnProductImages.AddRangeAsync(res);
                await _context.SaveChangesAsync();
            }
            return res;
        }

        [HttpGet("VendorProductRequestForReturn")]
        [Authorize(Roles = "Vendor")]
        // Seeing all of the vendor`s customer requests to the return product.
        public async Task<ActionResult<List<VendorReturnView>>> VendorProductRequestForReturn()
        {

            var user = await _userManager.GetUserAsync(User);

            var returnView = await (from p in _context.ReturnProducts
                                    where p.VendorId == user.Id
                                    select new VendorReturnView()
                                    {
                                        ReturnProductsId = p.Id,
                                        Amount = p.PlaceOrderItems.Amount,
                                        Price = p.PlaceOrderItems.Price,
                                        MovieName = p.PlaceOrderItems.Movie.Name,
                                        ConsumerId = p.ConsumerId,
                                        ConsumerName = p.Consumer.UserName,
                                        PlaceOrderId = p.PlaceOrderItemsId,
                                        ReturnRequestDate = p.DateRequest,
                                        Description = p.Description,
                                        Email = p.Consumer.Email,
                                        Reasons = p.ReturnReasons.Reason,
                                        ProductImages = (from st in p.ReturnProductImages
                                                         select new CustomerProductImages
                                                         {
                                                             ProductImage = st.ProductImage,
                                                             Username = st.Consumer.UserName
                                                         }).ToList()
                                    }).AsNoTracking().ToListAsync();
            return returnView;
        }

        [HttpGet("ReturnProductView/{returnProductId:int}")]
        public async Task<ActionResult<ReturnProductView>> ReturnProductView(int returnProductId)
        {
            var user = await _userManager.GetUserAsync(User);

            var returnView = await (from s in _context.ReturnProducts
                                    where s.VendorId == user.Id 
                                    where s.Id == returnProductId
                                    select new ReturnProductView
                                    {
                                        Amount = s.PlaceOrderItems.Amount,
                                        DatePurchase = s.PlaceOrderItems.PlacedOrderCreation,
                                        MovieId = s.PlaceOrderItems.MovieId,
                                        MovieName = s.PlaceOrderItems.Movie.Name,
                                        Price = s.PlaceOrderItems.Price,
                                        PlaceOrderItemsId = s.PlaceOrderItemsId,
                                        VendorName = s.Vendor.UserName
                                    }).FirstOrDefaultAsync();

            return returnView;
        }

  

        [HttpPut("ProductVendorDecision")]
        [Authorize(Roles = "Vendor")]
        // Vendor who owned the product and his customer has a return product request
        // It will just agreed or declined the request. If yes it just simply decrease the amount of sales
        // in that product
        public async Task<ActionResult> ProductVendorDecision([FromBody] VendorDecisionDTO d)
        {

            var user = await _userManager.GetUserAsync(User);

            // Make sure the user who agree and declined the returned product is the vendor
            // who owned the product.
            var returnProduct = await _context.ReturnProducts
                .Where(w=> w.VendorId == user.Id && w.PlaceOrderItems.VendorId == user.Id)
                .FirstOrDefaultAsync(w => w.Id == d.ReturnProductsId);

            var productTransaction = await _context.ProductTransactions
                .Where(w => w.VendorId == user.Id)
                .FirstOrDefaultAsync(w => w.PlaceOrderItemsId == returnProduct.PlaceOrderItemsId);

            var checkIfExists = await _context.ReturnProducts.AnyAsync(w => w.Id == d.ReturnProductsId);

            if (checkIfExists && returnProduct.VendorId == user.Id)
            {
                returnProduct.ReturnedProductApproved = d.Approved;
                 _context.ReturnProducts.Update(returnProduct);

                productTransaction.IsReturned = true;
                _context.ProductTransactions.Update(productTransaction);

                await _context.SaveChangesAsync();
            }
            return Ok();
        }
      

        [HttpPost("ReturnProductReason")]
        [AllowAnonymous]
        // Admin route
        public async Task<ActionResult> ReturnProductReasonCreation([FromBody] ReturnReasonCreationDTO dto)
        {
            var obj = new ReturnReasons()
            {
                Reason = dto.Reason
            };
            await _context.ReturnReasons.AddAsync(obj);
            await _context.SaveChangesAsync();
            return Ok();
        }


        
    }
}
