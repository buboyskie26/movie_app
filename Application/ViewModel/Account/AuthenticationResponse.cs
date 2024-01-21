using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Account
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public string RoleName { get; set; }
        public DateTime Expiration { get; set; }
    }
}
