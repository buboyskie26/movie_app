using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ReturnProduct
    {
        public int Id { get; set; }
        public List<ReturnProductImage> ReturnProductImages { get; set; }

        /*public string ProductImage { get; set; }*/
        public string Description { get; set; }
        public int PlaceOrderItemsId { get; set; }
        public ReturnReasons ReturnReasons { get; set; }
        public int ReturnReasonsId { get; set; }
        public PlaceOrderItems PlaceOrderItems { get; set; }
        public DateTime DateRequest { get; set; }
        public ApplicationUser Consumer { get; set; }
        public string ConsumerId { get; set; }
        public ApplicationUser Vendor { get; set; }
        public string VendorId { get; set; }
        public bool? ReturnedProductApproved { get; set; }
    }
}
