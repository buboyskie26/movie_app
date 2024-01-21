using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class CartConsumer
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string VendorId { get; set; }

        public ApplicationUser Consumer { get; set; }
        public string ConsumerId { get; set; }

        public DateTime DateCart { get; set; }

    }
}
