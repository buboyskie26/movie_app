using Application.ViewModel.MessageTable;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.IService
{
    public interface IMessageTable
    {
        Task<bool> CheckUserChatExists(string userId, MessageTableCreationDTO dto);
        Task AddingMessageUser(MessageTableCreationDTO dto, string userId,
            DateTime now, MessageTable messageTable);
        Task<MessageTable> AddingMessageTable(MessageTableCreationDTO dto, string userId,
            DateTime now);

        Task AddingMessageNotification(MessageTableCreationDTO dto,
            ApplicationUser user, DateTime now, int messageTableId, int messagesListCount);
        Task<MessageTable> GetMessageTable(MessageTableCreationDTO dto, string userId);
        Task<MessageNotification> GetNotificationTable(MessageTableCreationDTO dto, string userId,
            MessageTable getMessageTable);
        Task CreateUserReply(MessageTableCreationDTO dto, string userId, DateTime now, MessageTable getMessageTable);
        Task<List<MessageUsersTable>> GetMessageList(MessageTableCreationDTO dto, ApplicationUser user);
        Task AddingMessageUserTable(MessageTableCreationDTO dto, ApplicationUser user, DateTime now,
            MessageTable getMessageTable);
        Task<List<MessageUsersTable>> GetMessageTwo(MessageTableCreationDTO dto, ApplicationUser user);
        Task AddingMessageNotification(MessageTableCreationDTO dto, ApplicationUser user, DateTime now, MessageTable getMessageTable,
            int messagesListTwoCount);
        void UpdatingMessageNotifMessageBody(ApplicationUser user, DateTime now, MessageNotification getNotificationTable,
            List<MessageUsersTable> messagesList);
    }
}
