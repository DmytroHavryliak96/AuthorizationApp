using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApp.ViewModels.Validations;

namespace AuthorizationApp.ViewModels
{
    public class CredentialViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
