using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class OrderItems
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int OrdersId { get; set; }
        public Orders Orders { get; set; }
        public DateTime DateOrder { get; set; }

    }
}
