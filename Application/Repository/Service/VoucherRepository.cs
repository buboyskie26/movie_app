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
    public class VoucherRepository : IVoucher
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public VoucherRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Tuple<string, int>> CreateVoucherPost(VoucherCreationDTO dto, string userId)
        {
            var now = DateTime.Now;

            // Check if movie is yours.
            var ownedMovie = await _context.Vouchers
                .Include(w => w.Movie)
                .Where(w => w.VendorId == userId)
                .Where(w => w.MovieId == dto.MovieId)
                .AnyAsync();

            var voucher = new Voucher();
            var product = new Movie();

            // Check if the movie you`re selecting is yours
            if (ownedMovie == true)
            {
                voucher = new Voucher
                {
                    VendorId = userId,
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

               
                /*return Ok($"Successfully created an voucher for {product.Name} that has %{voucher.DiscountPercentage} discounted price");*/
            }
            /*var voucherNotif = new VoucherNotification();*/




            // Notify only who are certified customer of the vendor.
            var usersNotify = _context.ProductTransactions
                .Include(w => w.Consumer)
                .Where(w => w.VendorId == userId)
                .Where(w => w.VendorApproved == true)
                /*.Select(w=> w.Consumer.UserName)*/
                .Distinct()
                .ToList();

            if (usersNotify != null && usersNotify.Count != 0)
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
            if(ownedMovie == true)
            {
                var values = new Tuple<string, int>(product.Name, voucher.DiscountPercentage);
                return await Task.FromResult(values);
            }
            else
            {

                var errorMsg = new Tuple<string, int>("", 0);
                return await Task.FromResult(errorMsg);
            }

        }
    }
}
