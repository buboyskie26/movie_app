using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class ProductInformationDTO
    {
        public int MovieId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double Price { get; set; }
        public int TotalSold { get; set; }
        public string ProductAddress { get; set; }
    }
}
