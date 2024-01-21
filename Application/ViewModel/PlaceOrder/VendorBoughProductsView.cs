using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.PlaceOrder
{
    public class MovieProperties
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string MoviePicture { get; set; }
        public double MoviePrice { get; set; }
    }
    public interface PlaceOrderProperties
    {
        public int PlaceOrderId { get; set; }

    }
    public class PickupDetailsView : PlaceOrderProperties
    {
        public int PlaceOrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerContactNumber { get; set; }
        public string PickUpDate { get; set; }
    }

    public class VendorBoughProductsView : MovieProperties, PlaceOrderProperties
    {
        public string ConsumerName { get; set; }

        public int ProductTransactionId { get; set; }
        public int PlaceOrderId { get; set; }
        public int PlaceOrderShippingFee { get; set; }
        public double TotalAmount { get; set; }
        public string OrderTime { get; set; }
        public string RiderName { get; set; }
        public bool SuccessfullyDelivered { get; set; }

    }
}
