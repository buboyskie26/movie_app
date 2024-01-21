using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Movie
    {
        public int Id { get; set; }
        public int StockCount { get; set; }
        public int Sold { get; set; }
        /*public int RateReview { get; set; }*/
        public string Address { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImageURL { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        /*public MovieCategory MovieCategory { get; set; }
        public int ProducerId { get; set; }
        [ForeignKey("ProducerId")]
        public Producer Producer { get; set; }
        public int CinemaId { get; set; }
        [ForeignKey("CinemaId")]
        public Cinema Cinema { get; set; }

        public IEnumerable<Actor_Movie> Actor_Movie { get; set; }*/
        public IEnumerable<MovieUserView> MovieUserViews { get; set; }
        public ApplicationUser Vendor { get; set; }
        public string VendorId { get; set; }
        public string ProductCategory { get; set; }
        public int ShippingFee { get; set; }
        public int TotalReturnedProducts { get; set; }
        public int TotalStockCount { get; set; }
        public IEnumerable<Voucher> Vouchers { get; set; }
    }
}
