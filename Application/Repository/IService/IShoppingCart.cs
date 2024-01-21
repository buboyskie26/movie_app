using Application.BaseRepository;
using Application.Repository.IService.SubInterface;
using Application.ViewModel.ShoppingCart;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService
{
    public interface IShoppingCart : RefreshCartProcess, IOutOfStockNotification
    {
        Task PutChangeDiscount(string userId, AddItemToCartManyDTO dto);
        Task MessageProductNotificationRemoval(string userId);

    }
}
