using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MessageUsersTable
    {
        public int Id { get; set; }
        public MessageTable MessageTable { get; set; }
        public int MessageTableId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        // User whom created the messageTable would be UserOne
        public ApplicationUser UserOne { get; set; }
        public string UserOneId { get; set; }
        // User whom chatted by UserOne
        public ApplicationUser UserTwo { get; set; }
        public string UserTwoId { get; set; }
        public bool IsMessageRemove { get; set; }
        public DateTime? IsReadDate { get; set; }
        public DateTime? MessageCreated { get; set; }

    }
}
