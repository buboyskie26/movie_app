using Application.Repository.IService;
using Application.ViewModel.MessageTable;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Repository.Service
{
    public class MessageTableRepository : IMessageTable
    {
        private readonly ApplicationDbContext _context;

        public MessageTableRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddingMessageNotification(MessageTableCreationDTO dto, ApplicationUser user, DateTime now, int messageTableId, int messagesListCount)
        {
            var messageeNotification = new MessageNotification()
            {
                UserWhomMakeId = user.Id,
                ReceivingUserId = dto.UserIWillMessageId,
                Creation = now,
                MessageTableId = messageTableId,
                Header = $"{user.UserName} has message you",
                MessageBody = $"There`s a ({messagesListCount}) pending messages from {user.UserName}",
            };

            await _context.MessageNotifications.AddAsync(messageeNotification);
            await _context.SaveChangesAsync();
        }

        public async Task AddingMessageNotification(MessageTableCreationDTO dto, ApplicationUser user, DateTime now, MessageTable getMessageTable, int messagesListTwoCount)
        {
            var notif = new MessageNotification
            {
                UserWhomMakeId = user.Id,
                ReceivingUserId = dto.UserIWillMessageId,
                Creation = now,
                MessageTableId = getMessageTable.Id,
                Header = $"{user.UserName} has message you",
                MessageBody = $"There`s a ({messagesListTwoCount}) pending messages from {user.UserName}",

            };
            await _context.MessageNotifications.AddAsync(notif);
        }

        public async Task<MessageTable> AddingMessageTable(MessageTableCreationDTO dto, string userId, DateTime now)
        {
            var messageTable = new MessageTable()
            {
                MessageCreated = now,
                UserWhoStartMessageId = userId,
                UserHeHadMessageId = dto.UserIWillMessageId,
            };

            await _context.MessageTables.AddAsync(messageTable);
            await _context.SaveChangesAsync();

            return messageTable;
        }

        public async Task AddingMessageUser(MessageTableCreationDTO dto, string userId, DateTime now, MessageTable messageTable)
        {
           var userMessage = new MessageUsersTable
            {
                IsRead = false,
                MessageCreated = now,
                MessageTableId = messageTable.Id,
                UserOneId = userId,
                UserTwoId = dto.UserIWillMessageId,
                Message = dto.MessageBody
            };

            await _context.MessageUsersTables.AddAsync(userMessage);
            await _context.SaveChangesAsync();
        }

        public async Task AddingMessageUserTable(MessageTableCreationDTO dto, ApplicationUser user, DateTime now, MessageTable getMessageTable)
        {
            var messageUserTable = new MessageUsersTable
            {
                IsRead = false,
                MessageCreated = now,
                MessageTableId = getMessageTable.Id,
                UserOneId = user.Id,
                UserTwoId = dto.UserIWillMessageId,
                Message = dto.MessageBody
            };

            await _context.MessageUsersTables.AddAsync(messageUserTable);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckUserChatExists(string userId, MessageTableCreationDTO dto)
        {
            return await _context.MessageTables
                .Where(w => (w.UserWhoStartMessageId == userId
                  && w.UserHeHadMessageId == dto.UserIWillMessageId)
                  || (w.UserHeHadMessageId == userId && w.UserWhoStartMessageId == dto.UserIWillMessageId))
                .AnyAsync();
        }

        public async Task CreateUserReply(MessageTableCreationDTO dto, string userId, DateTime now, MessageTable getMessageTable)
        {
            var createUserReply = new MessageUsersTable
            {
                MessageTableId = getMessageTable.Id,
                UserOneId = userId,
                UserTwoId = dto.UserIWillMessageId,
                MessageCreated = now,
                Message = dto.MessageBody
            };

            await _context.MessageUsersTables.AddAsync(createUserReply);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MessageUsersTable>> GetMessageList(MessageTableCreationDTO dto, ApplicationUser user)
        {
            return await _context.MessageUsersTables
                            .Where(w => w.IsRead == false)
                            .Where(w => w.UserOneId == user.Id && w.UserTwoId == dto.UserIWillMessageId)
                            .ToListAsync();
        }

        public async Task<MessageTable> GetMessageTable(MessageTableCreationDTO dto, string userId)
        {
            return await _context.MessageTables
              .Where(w => (w.UserWhoStartMessageId == userId
                && w.UserHeHadMessageId == dto.UserIWillMessageId)
                || (w.UserHeHadMessageId == userId && w.UserWhoStartMessageId == dto.UserIWillMessageId))
              .FirstOrDefaultAsync();
        }

        public async Task<List<MessageUsersTable>> GetMessageTwo(MessageTableCreationDTO dto, ApplicationUser user)
        {
            return await _context.MessageUsersTables
                .Where(w => w.IsRead == false)
                .Where(w => w.UserOneId == user.Id && w.UserTwoId == dto.UserIWillMessageId)
                .ToListAsync();
        }

        public async Task<MessageNotification> GetNotificationTable(MessageTableCreationDTO dto, string userId, MessageTable getMessageTable)
        {
            return await _context.MessageNotifications
                .Where(w => w.IsRead == false)
                .Where(w => w.UserWhomMakeId == userId && w.ReceivingUserId == dto.UserIWillMessageId)
                .FirstOrDefaultAsync(w => w.MessageTableId == getMessageTable.Id);
        }

        public void UpdatingMessageNotifMessageBody(ApplicationUser user, DateTime now, MessageNotification getNotificationTable, List<MessageUsersTable> messagesList)
        {
            getNotificationTable.MessageBody = $"There`s a ({messagesList.Count}) pending messages from {user.UserName}";
            getNotificationTable.Creation = now;
            _context.MessageNotifications.Update(getNotificationTable);
        }
    }
}
