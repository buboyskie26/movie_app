using Application.ViewModel.Voucher;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService
{
    public interface IDiscountedShop
    {
        Task<bool> CreateMinimumSpendPost(DiscountedShopDTO dto, string userId);
    }
}
