using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class VoucherNotification
    {
        public int Id { get; set; }
        public Voucher Voucher { get; set; }
        public int VoucherId { get; set; }
        public DateTime Creation { get; set; }
        public string ProductImage { get; set; }
        public string Header { get; set; }
        public string MessageBody { get; set; }
        public ApplicationUser ReceivingUser { get; set; }
        public string ReceivingUserId { get; set; }
    }
}
