using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Account
{
    public class MyAccountView
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Fullname => FirstName + " " + LastName;
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageProfile { get; set; }

    }

}
