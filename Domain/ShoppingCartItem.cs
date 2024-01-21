using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public Movie Movie { get; set; }
        public int MovieId { get; set; }
        public int Amount { get; set; }
        public int ShippingFee { get; set; }
        public double Price { get; set; }
        public bool IsSelected { get; set; }
        public string MyCartUserId { get; set; }
        public ApplicationUser MyCartUser { get; set; }
        public ApplicationUser Vendor { get; set; }
        public string VendorId { get; set; }
        public bool IsCoupon { get; set; }
        public bool IsMinimumQuota { get; set; }
        public bool IsVoucher { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalDiscount { get; set; }
        public DateTime DateAddToCart { get; set; }
        public ICollection<ShoppingVoucher> ShoppingVouchers { get; set; } = new List<ShoppingVoucher>();
        public ICollection<DiscountShop_Cart> DiscountShop_Carts { get; set; } = new List<DiscountShop_Cart>();
    }
}
