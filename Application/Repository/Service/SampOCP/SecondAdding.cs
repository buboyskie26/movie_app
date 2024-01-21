using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service.SampOCP
{
    public class SecondAdding : IAddingBehavior
    {
        /*private readonly ApplicationDbContext _context;
         
        public SecondAdding(ApplicationDbContext context)
        {
            _context = context;
        }*/
        public ShoppingCartItem SampAddingMethod(ISampAddingProp prop)
        {
            var cart = new ShoppingCartItem();

            cart.MovieId = prop.Movie.Id;
            cart.Amount = 1;
            cart.TotalDiscount = prop.TotalDiscount;
            cart.MyCartUserId = prop.UserId;
            cart.DateAddToCart = prop.Now;
            cart.IsSelected = true;
            cart.IsMinimumQuota = true;
            cart.ShippingFee = prop.MovieShipping;
            cart.IsCoupon = false;
            cart.IsVoucher = true;
            cart.Price = (double)(prop.FinalPrice);
            cart.VendorId = prop.Movie.VendorId;
            // Adds
            cart.IsMinimumQuota = true;

            return cart;
            /*await _context.ShoppingCartItems.AddAsync(cart);
            await _context.SaveChangesAsync();*/
        }
    }
}
