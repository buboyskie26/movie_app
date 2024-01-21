using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class MovieCheckout
    {
        public List<int?> ShoppingCartId { get; set; }
    }


    public class ProductsOrderedList
    {
        public IEnumerable<MovieCheckoutList> ProductsOrdered { get; set; }
        /*public int ShippingTotal { get; set; }*/
        public double TotalPrice { get; set; }
    }

    public class MovieCheckoutList
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string UserName { get; set; }
/*        public string Address { get; set; }
        */
        public int Amount { get; set; }
        public double UnitPrice { get; set; }
        public double DiscountedPrice { get; set; }
    }
}
