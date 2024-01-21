using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MovieOutOfStockNotification
    {
        public int Id { get; set; }
        public MovieOutOfStock MovieOutOfStock { get; set; }
        public int MovieOutOfStockId { get; set; }
        public DateTime Creation { get; set; }
        public string ProductImage { get; set; }
        public string Header { get; set; }
        public string MessageBody { get; set; }
        public ApplicationUser ReceivingUser { get; set; }
        public string ReceivingUserId { get; set; }
    }
}
