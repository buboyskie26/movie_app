using Application.ViewModel.Voucher;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService
{
    public interface IVoucher
    {
        Task<Tuple<string, int>> CreateVoucherPost(VoucherCreationDTO dto, string userId);

    }
}
