using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Code { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }
    }
}
