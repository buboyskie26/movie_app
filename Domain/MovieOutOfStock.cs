using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class MovieOutOfStock
    {
        public int Id { get; set; }
        public Movie Movie { get; set; }
        public int MovieId { get; set; }
        public ApplicationUser UserAttemptToCartOutOfStock { get; set; }
        public string UserAttemptToCartOutOfStockId { get; set; }

         

        public DateTime DateCreation { get; set; }
        public ICollection<MovieOutOfStockNotification> MovieOutOfStockNotifications { get; set; } = new List<MovieOutOfStockNotification>();
        public bool IsClicked { get; set; }
        public bool IsOutOfStock { get; set; }

    }
}
