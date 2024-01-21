using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MessageNotification
    {
        public int Id { get; set; }
        public MessageTable MessageTable { get; set; }
        public int MessageTableId { get; set; }
        public DateTime Creation { get; set; }
        public string Header { get; set; }
        public string MessageBody { get; set; }
        public ApplicationUser ReceivingUser { get; set; }
        public string ReceivingUserId { get; set; }
        public ApplicationUser UserWhomMake { get; set; }
        public string UserWhomMakeId { get; set; }
        public bool IsRead { get; set; }
        public DateTime ReadTime { get; set; }

    }
}
