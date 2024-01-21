using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.BaseRepository
{
    public interface IBaseRepository<T>
    {
        Task<T> CheckExists(object id);
        Task Create(T t);
        Task CreateRange(T t);
        Task Update(object id, object model);
        Task<T> GetOne(object id);
        Task Delete(object id);
    }
}
