using Application.Repository.IService;
using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Service
{
    public class MessageNotificationRepository : IMessageNotification
    {
        private readonly ApplicationDbContext _context;
        public MessageNotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tuple<MessageTable, List<MessageTable>>> AddingProductMessageTable(DateTime now, List<ShoppingCartItem> shoppingCart)
        {
            var messageTable = new MessageTable();
            var messageTableList = new List<MessageTable>();

            foreach (var item in shoppingCart)
            {
                messageTable = new MessageTable()
                {
                    UserHeHadMessageId = item.MyCartUserId,
                    MessageCreated = now,
                    UserWhoStartMessageId = item.Movie.VendorId,
                    MovieRefId = item.MovieId
                };
                messageTableList.Add(messageTable);
            }
            await _context.MessageTables.AddRangeAsync(messageTableList);
            await _context.SaveChangesAsync();

            var values = new Tuple<MessageTable, List<MessageTable>>(messageTable, messageTableList);
            return await Task.FromResult(values);
        }

        public async Task AddProductMessageUserTable(DateTime now, MessageTable messageTable, List<MessageTable> messageTableList)
        {
            var vendorMessageGenerated = new MessageUsersTable();
            var vendorMessageGeneratedList = new List<MessageUsersTable>();

            foreach (var item in messageTableList)
            {
                if (messageTable != null)
                {
                    vendorMessageGenerated = new MessageUsersTable
                    {
                        IsRead = false,
                        MessageCreated = now,
                        MessageTableId = messageTable.Id,
                        UserOneId = item.UserWhoStartMessageId,
                        UserTwoId = item.UserHeHadMessageId,
                        Message = "Did you forget something?An item in your cart is selling out," +
                        " complete your purchase now!"
                    };
                }
                vendorMessageGeneratedList.Add(vendorMessageGenerated);
            }

            await _context.MessageUsersTables.AddRangeAsync(vendorMessageGeneratedList);
            await _context.SaveChangesAsync();
        }
    }
}
