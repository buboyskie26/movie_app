using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class PlaceOrderItems
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public int ShippingFee { get; set; }
        public double TotalPrice { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public ApplicationUser UserPlaceOrder { get; set; }
        // User who placed an order
        public string UserPlaceOrderId { get; set; }
        public DateTime PlacedOrderCreation { get; set; }

        // User who owned the shop.
        public ApplicationUser Vendor { get; set; }
        public string VendorId { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalDiscount { get; set; }


    }
}
