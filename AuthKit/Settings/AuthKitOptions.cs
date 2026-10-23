using System;
using System.Collections.Generic;
using System.Text;

namespace AuthKit.Settings
{
    public class AuthKitOptions
    {
       
        public bool SendConfirmationEmailOnRegister { get; set; }

      
        public bool RequireConfirmedEmail { get; set; }
    }
}
