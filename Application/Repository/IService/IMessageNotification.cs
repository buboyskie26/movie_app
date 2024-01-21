using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService
{
    public interface IMessageNotification
    {
        Task<Tuple<MessageTable, List<MessageTable>>> AddingProductMessageTable(DateTime now, List<ShoppingCartItem> shoppingCart);
        Task AddProductMessageUserTable(DateTime now, MessageTable messageTable,
            List<MessageTable> messageTableList);
    }
}
