using Application.Repository.IService;
using Application.ViewModel.Voucher;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service
{
    public class DiscountedShopRepository : IDiscountedShop
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public DiscountedShopRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> CreateMinimumSpendPost(DiscountedShopDTO dto, string userId)
        {
            var now = DateTime.Now;

            // Remove promotion if expires of quantity is already zero.
            var expiresPromotion = await _context.DiscountedShop
                .Where(w => w.VendorId == userId)
                .Where(w => w.Quantity == 0 || w.Expire <= now)
                .FirstOrDefaultAsync();

            if (expiresPromotion != null)
            {
                _context.DiscountedShop.Remove(expiresPromotion);
                await _context.SaveChangesAsync();
            }

            var checkIfAlreadyCreatedPromotion = await _context.DiscountedShop
                .Where(w => w.VendorId == userId)
                .AnyAsync();

            if (checkIfAlreadyCreatedPromotion == false)
            {
                var discountShopQuota = new DiscountedShop
                {
                    VendorId = userId,
                    Creation = now,
                    Expire = now.AddDays(3),
                    Quantity = dto.Quantity,
                    MinimumSpend = dto.MinimumSpendQuota,
                    FixedDiscount = dto.FixedPrice
                };

                await _context.DiscountedShop.AddAsync(discountShopQuota);
                await _context.SaveChangesAsync();

                /*return Ok($"Successfully created a minimum quota of {dto.MinimumSpendQuota} to your shop");*/
            }
            return checkIfAlreadyCreatedPromotion;
        }
    }
}
