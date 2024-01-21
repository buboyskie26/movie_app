using Application.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.ViewModel.Account
{
    public class VendorProductPurchasedView
    {
        public IEnumerable<MyProductInformation> MyProductInformations { get; set; }
    }
    public class VendorProductSalesView
    {
        public IEnumerable<ProductPurchaseHistory> ProductPurchaseHistories { get; set; }
        public double TotalSales { get; set; }
    }
    public class CommonProductInformation
    {
        public int MovieId { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public string MovieName { get; set; }
        public int AvailableProduct { get; set; }
    }
    public class MyProductInformation : CommonProductInformation
    {
        public string MovieImage { get; set; }
        public string ProductCategory { get; set; }
        public int Sales { get; set; }
        public List<UserWhoViewed> UserView { get; set; }
    }
    public class ProductPurchaseHistory : CommonProductInformation
    {
        public int ProductRate { get; set; }
        public DateTime DatePurchased { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string RiderName { get; set; }
        public string RiderId { get; set; }
    }
    public class GroupingProductPurchaseHistory
    {
   
        public DateTime DatePurchased { get; set; }
        public IEnumerable<ProductPurchaseHistory> Items { get; set; }
    }
    public class UserWhoViewed
    {
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public DateTime DateViewed { get; set; }
    }

    public class MyShopPaginationFilter
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationDTO PaginationDTO
        {
            get
            {
                return new PaginationDTO()
                {
                    Page = Page,
                    RecordsPerPage = RecordsPerPage
                };
            }
        }
        public string MovieName { get; set; }
        public string MovieCategory { get; set; }
        public bool TopSale { get; set; }

    }

    public class VendorPurchaseHistoryPaginate
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationDTO PaginationDTO
        {
            get
            {
                return new PaginationDTO()
                {
                    Page = Page,
                    RecordsPerPage = RecordsPerPage
                };
            }
        }
        /*public string DatePurchased { get; set; }*/
        public DateTime DatePurchased { get; set; }
        public string MovieName { get; set; }

    }
}
