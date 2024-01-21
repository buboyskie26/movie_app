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

    public class DiscountedShopController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDiscountedShop _discountedShop;
        public DiscountedShopController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IDiscountedShop discountedShop)
        {
            _context = context;
            _userManager = userManager;
            _discountedShop = discountedShop;
        }
        // Serves as promotion to the single product.
        // You can used the route for (4x times) every month. 
        [HttpPost]
        [Authorize(Roles = "Vendor")]
        public async Task<IActionResult> CreateMinimumSpend([FromBody] DiscountedShopDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);

            // If vendor successfully created a promotion.
            bool successfullyCreated = await _discountedShop.CreateMinimumSpendPost(dto, user.Id);

            if (successfullyCreated)
            {
                return Ok($"Successfully created a minimum quota of {dto.MinimumSpendQuota} to your shop");
            }

            /*var voucherNotif = new VoucherNotification();*/
            // Check if the movie you`re selecting is yours
            return BadRequest("Only onced could create a promotion");
        }
    }
}
