using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationApp.Auth;
using AuthorizationApp.Models;
using Newtonsoft.Json;

namespace AuthorizationApp.Helpers
{
    public class Tokens
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializersettings)
        {
            var response = new
            {
                id = identity.Claims.Single(c => c.Type.Equals("id")).Value,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
                expires_in = (int) jwtOptions.ValidFor.TotalSeconds
            };

            return JsonConvert.SerializeObject(response, serializersettings);
        }
    }
}
