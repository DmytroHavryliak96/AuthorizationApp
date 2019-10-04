using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AuthorizationApp.BL.Authentification
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "http://localhost:5000";
        private const string KEY = "mysecretkey!123";
        public const int LIFETIME = 1;
       // public static SymmetricSecurityKey 
    }
}
