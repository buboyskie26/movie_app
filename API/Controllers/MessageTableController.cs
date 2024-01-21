using Application.Repository.IService;
using Application.ViewModel.MessageTable;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageTableController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMessageTable _messageTable;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessageTableController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMessageTable messageTable)
        {
            _context = context;
            _userManager = userManager;
            _messageTable = messageTable;
        }

        [HttpPost]
        public async Task<ActionResult> CreatingMessageTableWithSomeone(
            [FromBody] MessageTableCreationDTO dto)
        {
            var user = await _userManager.GetUserAsync(User);
            var now = DateTime.Now;

            var ifUserToChatExists = await _messageTable.CheckUserChatExists(user.Id, dto);

            if (dto.UserIWillMessageId == user.Id) return BadRequest("Chatting yourself is not allowed");

            if (ifUserToChatExists == false)
            {
                var messageTable = await _messageTable.AddingMessageTable(dto, user.Id, now);

                /*var messageTable = new MessageTable()
                {
                    MessageCreated = now,
                    UserWhoStartMessageId = user.Id,
                    UserHeHadMessageId = dto.UserIWillMessageId,
                };

                await _context.MessageTables.AddAsync(messageTable);
                await _context.SaveChangesAsync();*/

                // Get All messages from me and to the other user whom i messaged at.
                /*var userMessage = new MessageUsersTable
                {
                    IsRead = false,
                    MessageCreated = now,
                    MessageTableId = messageTable.Id,
                    UserOneId = user.Id,
                    UserTwoId = dto.UserIWillMessageId,
                    Message = dto.MessageBody
                };
                 
                await _context.MessageUsersTables.AddAsync(userMessage);
                await _context.SaveChangesAsync();*/

                await _messageTable.AddingMessageUser(dto, user.Id, now, messageTable);

                var messagesList = await _context.MessageUsersTables
                    .Where(w => w.UserOneId == user.Id && w.UserTwoId == dto.UserIWillMessageId)
                    .ToListAsync();

                if (messagesList != null)
                {
                    await _messageTable.AddingMessageNotification(dto, user, now,
                        messageTable.Id, messagesList.Count);

                    /*var messageeNotification = new MessageNotification()
                    {
                        UserWhomMakeId = user.Id,
                        ReceivingUserId = dto.UserIWillMessageId,
                        Creation = now,
                        MessageTableId = messageTable.Id,
                        Header = $"{user.UserName} has message you",
                        MessageBody = $"There`s a ({messagesList.Count}) pending messages from {user.UserName}",
                    };

                    await _context.MessageNotifications.AddAsync(messageeNotification);
                    await _context.SaveChangesAsync();*/

                }
                return Ok("Successfully message user that creates new message table");

            }
            // chat that were already had a message table
            else if (ifUserToChatExists == true)
            {
                var getMessageTable = await _messageTable.GetMessageTable(dto, user.Id);
                var getNotificationTable = await _messageTable.GetNotificationTable(dto, user.Id, getMessageTable);

                if (getNotificationTable != null)
                {
                    await _messageTable.CreateUserReply(dto, user.Id, now, getMessageTable);

                }
                var messagesList = await _messageTable.GetMessageList(dto, user);

                if (messagesList != null && getNotificationTable != null)
                {
                    _messageTable.UpdatingMessageNotifMessageBody(user, now, getNotificationTable,
                        messagesList);

                }
                // For User Two whom chatted by user one and user two wanted to chat user one.
                else if (getNotificationTable == null)
                {
                    await _messageTable.AddingMessageUserTable(dto, user, now, getMessageTable);

                    var messagesListTwo = await _messageTable.GetMessageTwo(dto, user);

                    // What if userTwo had replied to the user One who chatted him first. 
                    await _messageTable.AddingMessageNotification(dto, user, now, getMessageTable,
                        messagesListTwo.Count);
                }

                await _context.SaveChangesAsync();
                return Ok("Successfully reply to someone");
            }

            // If the user who you had chatted. And Checked the message. The existing notification aligned to him
            // would be deleted.

            return Ok();
            /*return Ok($"Successfully starts a message to {userIHadMessage.UserHeHadMessage.FirstName} {userIHadMessage.UserHeHadMessage.LastName}");*/
        }

        private void UpdatingMessageNotifMessageBody(ApplicationUser user, DateTime now, MessageNotification getNotificationTable, List<MessageUsersTable> messagesList)
        {
            getNotificationTable.MessageBody = $"There`s a ({messagesList.Count}) pending messages from {user.UserName}";
            getNotificationTable.Creation = now;
            _context.MessageNotifications.Update(getNotificationTable);
        }

        private async Task AddingMessageNotification(MessageTableCreationDTO dto, ApplicationUser user, DateTime now, MessageTable getMessageTable, int messagesListTwoCount)
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
        private async Task<List<MessageUsersTable>> GetMessageTwo(MessageTableCreationDTO dto, ApplicationUser user)
        {
            return await _context.MessageUsersTables
                .Where(w => w.IsRead == false)
                .Where(w => w.UserOneId == user.Id && w.UserTwoId == dto.UserIWillMessageId)
                .ToListAsync();
        }
        private async Task AddingMessageUserTable(MessageTableCreationDTO dto, ApplicationUser user, DateTime now, MessageTable getMessageTable)
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
        private async Task<List<MessageUsersTable>> GetMessageList(MessageTableCreationDTO dto, ApplicationUser user)
        {
            return await _context.MessageUsersTables
                             .Where(w => w.IsRead == false)
                             .Where(w => w.UserOneId == user.Id && w.UserTwoId == dto.UserIWillMessageId)
                             .ToListAsync();
        }
        private async Task CreateUserReply(MessageTableCreationDTO dto, string userId, DateTime now, MessageTable getMessageTable)
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
        private async Task<MessageNotification> GetNotificationTable(MessageTableCreationDTO dto, string userId, MessageTable getMessageTable)
        {
            // Get All messages from me and to the other user whom i had messaged .
            return await _context.MessageNotifications
                .Where(w => w.IsRead == false)
                .Where(w => w.UserWhomMakeId == userId && w.ReceivingUserId == dto.UserIWillMessageId)
                .FirstOrDefaultAsync(w => w.MessageTableId == getMessageTable.Id);
        }
        private async Task<MessageTable> GetMessageTable(MessageTableCreationDTO dto, string userId)
        {
            return await _context.MessageTables
              .Where(w => (w.UserWhoStartMessageId == userId
                && w.UserHeHadMessageId == dto.UserIWillMessageId)
                || (w.UserHeHadMessageId == userId && w.UserWhoStartMessageId == dto.UserIWillMessageId))
              .FirstOrDefaultAsync();
        }

        [HttpGet("GetChatWithSomeone/{messageTableId}")]
        public async Task<ActionResult<List<GetChatWithSomeoneView>>> GetChatWithSomeone(int messageTableId)
        {
            var user = await _userManager.GetUserAsync(User);

            // Update if the user already seen the message

            var messageTable = await _context.MessageUsersTables
                .Where(w => w.UserOneId != user.Id && w.UserTwoId == user.Id)
                .Where(w => w.MessageTableId == messageTableId && w.IsRead == false)
                .ToListAsync();

            if (messageTable != null)
            {

                _context.AttachRange(messageTable);
                messageTable.ForEach(e =>
                {
                    e.IsRead = true;
                    e.IsReadDate = DateTime.Now;
                });
                /*messageTable.IsRead = true;
                messageTable.IsReadDate = DateTime.Now;*/
                await _context.SaveChangesAsync();

            }
            // Only user who sent message by UserOneId
            var conversation = await (from m in _context.MessageUsersTables

                                      where m.MessageTableId == messageTableId
                                      where m.UserTwoId == user.Id || m.UserOneId == user.Id
                                      select new GetChatWithSomeoneView
                                      {
                                          IsRead = m.IsRead,
                                          MessageCreate = m.MessageCreated.HasValue ? m.MessageCreated.Value.ToString("dd/M/yyyy HH:mm tt") : "n/a",
                                          MessageBody = m.IsMessageRemove == true ? "You Unsent a message" : m.Message,
                                          MyMessageUserId = m.UserOneId == user.Id ? m.UserOneId : m.UserOneId,
                                          MessageBy = m.UserOneId == user.Id ? m.UserOne.UserName : m.UserOne.UserName,
                                          MessageTableId = m.MessageTableId,
                                          IsMe = m.UserOneId == user.Id,
                                      }).AsNoTracking().ToListAsync();

            // If the user who you had chatted. And Checked the message.
            // The existing notification aligned to him would be deleted.
            var messageNotification = await _context.MessageNotifications
                .Where(w=> w.ReceivingUserId == user.Id)
                .FirstOrDefaultAsync(w => w.MessageTableId == messageTableId);
            if (messageNotification != null)
            {
                _context.MessageNotifications.Remove(messageNotification);
                await _context.SaveChangesAsync();
            }

            return conversation;
        }

        [HttpPut("MessageRemoving/{messageUserTableId}")]
        public async Task<ActionResult<List<GetChatWithSomeoneView>>> MessageRemoving([FromBody] int messageUserTableId)
        {
            var user = await _userManager.GetUserAsync(User);

            var messageNotification = await _context.MessageUsersTables
                .Where(w=> w.UserOneId == user.Id)
               .FirstOrDefaultAsync(w => w.Id == messageUserTableId);

            if (messageNotification != null)
            {
                messageNotification.IsMessageRemove = true;
                await _context.SaveChangesAsync();
                return Ok($"Message Removed '{messageNotification.Message}'");
            }
            else if (messageNotification == null)
            {
                return BadRequest("Only your message could be removed by yourself");
            }
            return BadRequest("Removing message went wrong.");
        }
    }
}
