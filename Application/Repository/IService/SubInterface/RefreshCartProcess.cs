using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService.SubInterface
{
    public interface RefreshCartProcess
    {
        Task RefreshCart(string userId);
        Task RefreshCart(string userId, List<int?> shoppingCartId);
    }
}
