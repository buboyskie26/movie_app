using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class DiscountedShop
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int MinimumSpend { get; set; }
        public int FixedDiscount { get; set; }
        public ApplicationUser Vendor { get; set; }
        public string VendorId { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Expire { get; set; }

        public ICollection<DiscountShop_Cart> DiscountShop_Carts { get; set; } = new List<DiscountShop_Cart>();

    }
}
