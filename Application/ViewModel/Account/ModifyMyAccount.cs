using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Account
{
    public class ModifyMyAccount
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string LastName { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
