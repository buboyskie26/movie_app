using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class MovieDashboardView
    {
        public MovieProduct MovieProducts { get; set; }
        public IEnumerable<ProductRateReview> ProductRateReviews { get; set; }
        public IEnumerable<ProductVouchers> ProductVoucher { get; set; }
        public double OverallRating { get; set; }
    }
    public class ProductVouchers
    {
        public int MovieId { get; set; }
        public int VoucherId { get; set; }
        public int DiscountPrice { get; set; }
        public int MinimumSpend { get; set; }
    }
    public class ProductRateReview
    {
        public int MovieId { get; set; }
        public int RateCount { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public string DateCreated { get; set; }
    }
    public class MovieProductView
    {
        public ICollection<MovieProduct> MovieProductViews { get; set; }
        public ICollection<TrendingMovieSearches> TrendingMovieSearch { get; set; }
        public ICollection<RecommendedProduct> RecommendedProducts { get; set; }
        public ICollection<FlashSale> FlashSales { get; set; }

    }
    public class FlashSale : MovieProperties
    {
        public int VoucherId { get; set; }
        public int Discount { get; set; } 
        public double OriginalPrice { get; set; }
        public double DiscountedPrice { get; set; }
        public int Sold { get; set; }

    }

    public class TimePeriod
    {
        private double _seconds;

        public double Hours
        {
            get { return _seconds / 3600; }
            set
            {
                if (value < 0 || value > 24)
                    throw new ArgumentOutOfRangeException(nameof(value),
                          "The valid range is between 0 and 24.");

                _seconds = value * 3600;
            }
        }
    }
    public class MovieProperties
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string MoviePicture { get; set; }
    }
    public class TrendingMovieSearches : MovieProperties
    {
        public string VendorName { get; set; }
        public DateTime DateViewed { get; set; }
    }
    public class RecommendedProduct : MovieProperties
    {
        public string VendorName { get; set; }
        public int Sold { get; set; }
        public string VendorId { get; set; }
        public string ProductCategory { get; set; }
        public DateTime DateViewed { get; set; }
    }
    public class MovieProduct : MovieProperties
    { 
        public int SoldCount { get; set; }
        public int StockCount { get; set; }
        public string Address { get; set; }
        public string MovieDescription { get; set; }
        public int SoldItem { get; set; }
    }

}
