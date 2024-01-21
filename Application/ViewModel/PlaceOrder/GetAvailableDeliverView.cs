using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.PlaceOrder
{
    public class GetAvailableDeliverView
    {
        public int ProductTransactionId { get; set; }
        public string VendorName { get; set; }
        public string LocationToPickupParcel { get; set; }
        public string ConsumerName { get; set; }
        public string LocationToDeliverParcel { get; set; }
        public bool IsDelivered { get; set; }

    }
}
