using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace AuthorizationApp.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimsIdentifiers
            {
                public const string Rol = "rol", Id = "id";

            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
            }
        }
    }
}
