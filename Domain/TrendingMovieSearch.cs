using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class TrendingMovieSearch
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public DateTime DateViewed { get; set; }
        public int ViewedTimes { get; set; }
        public string ProductCategory { get; set; }
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
    }
}
