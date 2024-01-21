using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class RateProduct
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int RateCount { get; set; }
        public bool IsEnableToComment { get; set; }
        public DateTime RateCreation { get; set; }
        public DateTime RateEndedDate { get; set; }
        public string UserWhoHadRateId { get; set; }
        public ApplicationUser UserWhoHadRate { get; set; }
    }
}
