using Application.ViewModel.ReturnRequest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService
{
    public interface IReturnProduct
    {
        Task ReturnRequestProduct(ReturnRequestCreationDTO dto, string userId, string userName);

    }
}
