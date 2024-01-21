using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.ShoppingCart
{
    public class AddItemToCartDTO
    {
        public int MovieId { get; set; }
        public string VoucherCode { get; set; }
    }
    public class AddItemToCartManyDTO
    {
        public int MovieId { get; set; }
        public List<string> VoucherCode { get; set; }
    }
}
