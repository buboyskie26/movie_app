using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService.SubInterface
{
    public interface IOutOfStockNotification
    {
        Task UserLoginMovieOutOfStockNotify(int movieId, ApplicationUser user, DateTime now, Movie movie);
        Task<bool> MovieOutOfStockUserExistsNotif(string userId, int movieId);
        Task<bool> MovieOutOfStockUserExistsNotif(string userId, List<int> movieIds);
    }
}
