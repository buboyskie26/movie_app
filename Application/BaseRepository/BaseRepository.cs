using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _table;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }
       
        public async Task<T> CheckExists(object id)
        {
            return await _table.FindAsync(id);
        }
        public async Task Create(T t)
        {
            await _table.AddAsync(t);
            await _context.SaveChangesAsync();
        }

        public async Task CreateRange(T t)
        {
            await _table.AddRangeAsync(t);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(object id)
        {
            var t = await GetOne(id);
            if (t != null)
            {
                _table.Remove(t);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<T> GetOne(object id)
        {
            return await _table.FindAsync(id);
        }

        public async Task Update(object id, object model)
        {
            var t = await GetOne(id);
            if (t != null)
            {
                _context.Entry(t).CurrentValues.SetValues(model);
                await _context.SaveChangesAsync();
            }
        }
    }
}
