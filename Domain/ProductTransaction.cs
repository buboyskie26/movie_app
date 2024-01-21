using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ProductTransaction
    {
        public int Id { get; set; }
        public List<ProductTransactionResponse> ProductTransactionResponse { get; set; }
        public PlaceOrderItems PlaceOrderItems { get; set; }
        public int PlaceOrderItemsId { get; set; }
        public ApplicationUser Vendor { get; set; }
        public string VendorId { get; set; }
        public ApplicationUser Consumer { get; set; }
        public string ConsumerId { get; set; }
        public ApplicationUser Rider { get; set; }
        public string RiderId { get; set; }
        public bool IsOkayForDeliver { get; set; }
        public bool OrderPlaced { get; set; }
        public bool PaymentConfirmed { get; set; }
        public bool IsReturned { get; set; }
        public bool OrderReceived { get; set; }
        public bool ToRate { get; set; }
        public int Rate { get; set; }
        public int RateId { get; set; }
        public DateTime DateReceived { get; set; }
        public DateTime PickupDate { get; set; }
        public bool VendorApproved { get; set; }


    }
}
