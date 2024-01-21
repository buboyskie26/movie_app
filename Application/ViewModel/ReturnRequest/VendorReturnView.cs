using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.ReturnRequest
{
    public class VendorReturnView
    {
        public int ReturnProductsId { get; set; }
        public int PlaceOrderId { get; set; }
        public string MovieName { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public string ConsumerId { get; set; }
        public string ConsumerName { get; set; }
        public DateTime ReturnRequestDate { get; set; }
        public string Reasons { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public IEnumerable<CustomerProductImages> ProductImages { get; set; }
    }

    public class CustomerProductImages
    {
        public string ProductImage { get; set; }
        public string Username { get; set; }
    }
}
