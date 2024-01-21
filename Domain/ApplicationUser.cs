using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string ImageUrl { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Address { get; set; }

        public ICollection<PlaceOrderItems> Vendors { get; set; }
    }
}
