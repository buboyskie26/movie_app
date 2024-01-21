using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Account
{
    public class ProductNotificationView
    {
        public string Header { get; set; }
        public string Message { get; set; }
        public string Picture { get; set; }
        public string Creation { get; set; }
        public string Path { get; set; }
        /*public string PathId { get; set; }*/
    }
    public class ProductOutOfStockNotificationView : ProductNotificationView
    {
        public int MovieOutOfStockId { get; set; }
        
    }
}
