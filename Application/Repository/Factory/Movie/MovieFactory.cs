using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Factory.Movie
{
    public interface IMovieBehavior
    {
        Task MovieBehavior(bool one, bool two, string userId, int movieId);
    }
    public class MovieFactory : IMovieBehavior
    {
        private readonly ApplicationDbContext _context;
        public async Task MovieBehavior(bool one, bool two,string userId, int movieId)
        {
            var movieOutOfStock = await _context.MovieOutOfStocks
                .Where(w => w.UserAttemptToCartOutOfStockId == userId)
                /*.Where(w=> w.IsOutOfStock == false)*/
                .FirstOrDefaultAsync(w => w.MovieId == movieId);


            if (one && two)
            {
                var dateClick = movieOutOfStock?.DateCreation;
                // once you clicked, it will redirect to product itself
                movieOutOfStock.IsClicked = true;

                /*context.MovieOutOfStocks.Remove(movieOutOfStock);*/
                await _context.SaveChangesAsync();

                /*return Ok("Movie out of stock clicked.");*/

            }
            if (one && two == false)
            {

                _context.MovieOutOfStocks.Remove(movieOutOfStock);
                await _context.SaveChangesAsync();

                /*return Ok("Movie out of stock notification is now deleted.");*/

            }

            throw new NotImplementedException();
        }
    }
}
