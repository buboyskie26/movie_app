using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.ViewModel.PlaceOrder
{
    public class RatePurchasedOrder
    {
        [Range(0, 5)]
        public int Rate { get; set; }
        public int MovieId { get; set; }
        public string Message { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
