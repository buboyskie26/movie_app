using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
        public int DiscountPercentage { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public ApplicationUser Vendor { get; set; }
        public string VendorId { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Expire { get; set; }
        public IEnumerable<VoucherNotification> VoucherNotifications { get; set; }
        public IEnumerable<ShoppingVoucher> ShoppingVouchers { get; set; }
        public bool IsRemoved { get; set; }

    }
}
