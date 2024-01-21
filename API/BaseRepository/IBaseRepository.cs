using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.BaseRepository
{
    public interface IBaseRepository<T>
    {
        Task<T> CheckExists(object id);
        Task Create(T t);
        Task CreateRange(T t);
        Task Update(object id, object model);
        Task<T> GetOne(object id);
        Task Delete(object id);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    }
}
