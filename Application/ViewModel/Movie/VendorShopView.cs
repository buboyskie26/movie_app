using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class VendorShopView
    {
        public VendorSummary VendorSummary { get; set; }
        public IEnumerable<ProductFilteredByCategory> TopProducts { get; set; }

        public IEnumerable<GroupProductFilteredByCategory> GroupProductFilteredByCategories { get; set; }
    }
    public class VendorSummary
    {
        public int MovieId { get; set; }
        public int TotalProducts { get; set; }
        public string VendorName { get; set; }
        public double TotalRating { get; set; }
        public List<string> ProductCategory { get; set; }
    }
    public class GroupProductFilteredByCategory
    {
        public string CategoryTopic { get; set; }
        public IEnumerable<ProductFilteredByCategory> ProductFilteredByCategories { get; set; }

    }
    public class ProductFilteredByCategorySamp /*: ProductInformationDTO*/
    {
        public int MovieId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double Price { get; set; }
        public int TotalSold { get; set; }
        public string ProductAddress { get; set; }
        public string CategoryTopic { get; set; }
    }
    public class ProductFilteredByCategory /*: ProductInformationDTO*/
    {
        public int MovieId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public double Price { get; set; }
        public int TotalSold { get; set; }
        public string ProductAddress { get; set; }
        public string CategoryTopic { get; set; }
    }

/*    public class GroupedProductFilteredByCategory
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public IGrouping<string, ProductFilteredByCategory> Items { get; set; }
    }*/


}
