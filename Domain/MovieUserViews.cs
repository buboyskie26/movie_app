using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MovieUserView
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public DateTime ViewDate { get; set; }
        public Movie Movie { get; set; }
        public string UserWhoViewId { get; set; }
        public ApplicationUser UserWhoView { get; set; }
        public int ViewedTimes { get; set; }
    }
}
