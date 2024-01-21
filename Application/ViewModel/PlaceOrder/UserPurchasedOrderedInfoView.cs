using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.PlaceOrder
{
    public class UserPurchasedOrderedInfoView
    {
        public OrderProgression OrderProgressionView { get; set; }
        public PurchasedOrder PurchasedOrders { get; set; }
        public bool Returnable { get; set; }


    }

    public class OrderProgression
    {
        public int OrderId { get; set; }
        public string DeliveryAddress { get; set; }
        public string PhoneNumber { get; set; }
        public bool OrderPlaced { get; set; }
        public bool PaymentConfirmed { get; set; }
        public bool OrderReceived { get; set; }
        public bool ToRate { get; set; }
        public IEnumerable<TransactionResponse> TransactionResponses { get; set; }
    }
    public class TransactionResponse
    {
        public int ResponseId { get; set; }
        public string Message { get; set; }
        public DateTime DateCreation { get; set; }
    }
    public class PurchasedOrder
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string ImageUrl { get; set; }
        public int Amount { get; set; }
        public double Total { get; set; }
        public DateTime DatePurchased { get; set; }

    }
}
