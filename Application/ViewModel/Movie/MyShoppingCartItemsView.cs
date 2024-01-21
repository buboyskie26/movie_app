using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class MyShoppingCartItemsInnerView
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public double SubTotal { get; set; }
        public int SelectedAmount { get; set; }
        public double DiscountedPrice { get; set; }
        public double Price { get; set; }
        public bool IsSelected { get; set; }
    }
    public class MyShoppingCartItemsView
    {
        public IEnumerable<MyShoppingCartItemsInnerView> CartListItems { get; set; }
        public double Total { get; set; }
    }

}
