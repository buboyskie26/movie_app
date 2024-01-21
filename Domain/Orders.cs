
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Orders
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public List<OrderItems> OrderItems { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
