using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MessageTable
    {
        public int Id { get; set; }
        public ApplicationUser UserWhoStartMessage { get; set; }
        public string UserWhoStartMessageId { get; set; }
        public ApplicationUser UserHeHadMessage{ get; set; }
        public string UserHeHadMessageId { get; set; }
        public DateTime? MessageCreated { get; set; }
        public ICollection<MessageUsersTable> MessageUsersTables { get; set; } = new List<MessageUsersTable>();
        public int MovieRefId { get; set; }

    }
}
