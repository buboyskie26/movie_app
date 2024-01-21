using Application.Repository.IService;
using Application.ViewModel.Voucher;
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
    public class VoucherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IVoucher _voucher;
        private readonly UserManager<ApplicationUser> _userManager;

        public VoucherController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IVoucher voucher)
        {
            _context = context;
            _userManager = userManager;
            _voucher = voucher;
        }
        // Serves as promotion to the single product.
        // You can used the route for (4x times) every month. 
        [HttpPost]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateVoucherd([FromBody] VoucherCreationDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var now = DateTime.Now;

            // Check if movie is yours.
            var checkIfMyMovie = await _context.Vouchers
                .Include(w => w.Movie)
                .Where(w => w.VendorId == user.Id)
                .Where(w => w.MovieId == dto.MovieId)
                .AnyAsync();

            var voucher = new Voucher();
            var product = new Movie();

            // Check if the movie you`re selecting is yours
            if (checkIfMyMovie == true)
            {
                voucher = new Voucher
                {
                    VendorId = user.Id,
                    Creation = now,
                    DiscountPercentage = dto.DiscountPercentage,
                    Expire = now.AddDays(3),
                    MovieId = dto.MovieId,
                    Quantity = dto.Quantity,
                    Code = dto.VoucherCode == "string" || dto.VoucherCode == "" ? null : dto.VoucherCode,

                };

                await _context.Vouchers.AddAsync(voucher);
                await _context.SaveChangesAsync();

                product = _context.Movies
                    .Include(w => w.Vendor)
                    .FirstOrDefault(w => w.Id == voucher.MovieId);

                return Ok($"Successfully created an voucher for {product.Name} that has %{voucher.DiscountPercentage} discounted price");
            }
            // Notify only who are certified customer of the vendor.
            var usersNotify = _context.ProductTransactions
                .Include(w => w.Consumer)
                .Where(w => w.VendorId == user.Id)
                .Where(w => w.VendorApproved == true)
                /*.Select(w=> w.Consumer.UserName)*/
                .Distinct()
                .ToList();

            if(usersNotify != null && usersNotify.Count != 0)
            {
                var voucherNotif = (from userName in usersNotify
                                    select new VoucherNotification
                                    {
                                        VoucherId = voucher.Id,
                                        Creation = now,
                                        ReceivingUserId = userName.ConsumerId,
                                        Header = $"Alert Sale! {product.Vendor.UserName} has launch {product.Name} with a great discounted price.",
                                        ProductImage = product.ImageURL,
                                        MessageBody = $"Hi {userName.Consumer.UserName}, a new product with comes a huge discount is now launched. Hurry and look before it gone."
                                    }).ToList();

                await _context.VoucherNotifications.AddRangeAsync(voucherNotif);
                await _context.SaveChangesAsync();
            }
            


            return BadRequest("Creating voucher went wrong");
        }

        [HttpPost("CreateVoucher")]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherCreationDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);

            var (productName, voucherDiscount) = await _voucher.CreateVoucherPost(dto, user.Id);

            if(productName != "")
            {
                return Ok($"Successfully created an voucher for {productName} that has %{voucherDiscount} discounted price");
            }
            return BadRequest("Creating voucher went wrong");
        }

    }
}
