using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OvulaeShared.Enums.Auth;

namespace OvulaeApp.Models.Authentication
{
    public class PasswordReset
    {
        public PasswordResetType Type { get; set; }

        public string Email { get; set; }

        public string CountryCode { get; set; }

        public string PhoneNumber { get; set; }

        public string OTP { get; set; }
    }
}
