using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class MovieCreationDTO
    {
        [Display(Name = "Movie name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Movie description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Price in $")]
        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }

        [Display(Name = "Movie poster URL")]
        [Required(ErrorMessage = "Movie poster URL is required")]
        public IFormFile ImageURL { get; set; }
        [Required]
        public string ProductCategory { get; set; }

        public int Stocks { get; set; }
        public int ShippingFee { get; set; }
    }
    public class MovieStockDTO
    {
        public int MovieId { get; set; }
        public int StockNumber { get; set; }
    }
}
