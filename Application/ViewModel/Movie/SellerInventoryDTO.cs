using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class SellerInventoryDTO
    {
        public ICollection<SalesOnFirstSevenDay> SalesOnFirstQuarter { get; set; }
        public ICollection<SalesOnFirstSevenDay> SalesOnSecondQuarter { get; set; }
        public ICollection<SalesOnFirstSevenDay> SalesOnThirdQuarter { get; set; }
        public ICollection<SalesOnFirstSevenDay> SalesOnFourthQuarter { get; set; }
    }
    public class SalesOnFirstSevenDay : MovieProperties
    {
        public int Rating { get; set; }
        public int NumberOfReturned { get; set; }
        public double Sales { get; set; }
        public decimal Discount { get; set; }
    }
}
