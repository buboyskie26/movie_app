using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.PlaceOrder
{
    public class MyPurchaseProduct
    {
        public int PlaceOrderItemsId { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public DateTime DatePurchase { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
    }
    public class VendorShop
    {
        public int TotalProducts { get; set; }
        public int OverAllRating { get; set; }
        public string MovieName { get; set; }
        public string RateCount { get; set; }
        public int SoldCount { get; set; }
        public int StockCount { get; set; }
    }
}
