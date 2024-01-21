using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Account
{
    public class ToPayView
    {
        public int PlaceOrderId { get; set; }
        public int TransactionId { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public string RiderName { get; set; }
        public string RiderId { get; set; }
        public int ExpectedDayToDeliver { get; set; }


    }
}
