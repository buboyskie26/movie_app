using Application.ViewModel.Account;
using Application.ViewModel.Movie;
using Application.ViewModel.PlaceOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService
{
    public interface IAccount
    {
        Task<MyAccountView> MyAccount(string userId);
        Task<List<ToPayView>> ToPayView(string userId);
        Task EditMyAccount(ModifyMyAccount accountEditDTO, string userId);
        Task<List<MyPurchaseProduct>> PurchaseHistory(string userId);
        Task<VendorShopView> VendorShop(string userId);

    }
}
