using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Cart
{
    public class ShoppingCart
    {
        private readonly ApplicationDbContext _context;
        public string ShoppingCartId { get; set; }

        public ShoppingCart(ApplicationDbContext context)
        {
            _context = context;
        }
        public static ShoppingCart GetShoppingCart(IServiceProvider service)
        {
            ISession session = service.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            var context = service.GetService<ApplicationDbContext>();

            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", cartId);
 
            return new ShoppingCart(context) { ShoppingCartId = cartId };

        }
        public async Task AddItemToCart(int movieId)
        {
            var cart = new ShoppingCartItem();
            var checkMovieExists = await _context.Movies.AnyAsync(w => w.Id == movieId);

            if (checkMovieExists == true)
            {
                var movie = await _context.Movies
                              .FirstOrDefaultAsync(w => w.Id == movieId);

                var cartMovieExists = _context.ShoppingCartItems
                    .FirstOrDefault(w => w.MovieId == movieId  );

                if (cartMovieExists != null)
                {
                    // Add amount
                    cartMovieExists.Amount += 1;
                    _context.ShoppingCartItems.Update(cartMovieExists);

                }
                else if (cartMovieExists == null)
                {
                    // New item from your cart.
                    cart.MovieId = movie.Id;
                    cart.Amount = 1;
                    /*cart.ShoppingCartId = ShoppingCartId;*/
                    await _context.ShoppingCartItems.AddAsync(cart);

                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
