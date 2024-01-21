using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service
{
    public class Utils
    {
        private readonly ApplicationDbContext _context;
        public Utils(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> MovieOutOfStockUserExistsNotif(string userId, int movieId)
        {
            return await _context.MovieOutOfStocks
                        .Where(w => w.UserAttemptToCartOutOfStockId == userId)
                        .AnyAsync(w => w.MovieId == movieId);

        }
    }
}
