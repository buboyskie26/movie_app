using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.MessageTable
{
    public class MessageTableCreationDTO
    {
        public string UserIWillMessageId { get; set; }
        [MaxLength(50)]
        public string MessageBody { get; set; }
    }
    public class GetChatWithSomeoneView
    {
        public int MessageTableId { get; set; }
        public string MyMessageUserId { get; set; }
        public string MessageBy { get; set; }
        public string MessageBody { get; set; }
        public string MessageCreate { get; set; }
        public bool? IsRead { get; set; }
        public bool IsMe { get; set; }


    }
}
