using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApp.Auth;
using AuthorizationApp.Helpers;
using AuthorizationApp.Models;
using AuthorizationApp.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthorizationApp.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IJwtFactory jwtFactory;
        private readonly JwtIssuerOptions jwtOptions;

        public JwtTokenService(IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            this.jwtFactory = jwtFactory;
            this.jwtOptions = jwtOptions.Value;
        }

        public async Task<string> GetJwtToken(AppUser identity)
        {
            var jwt = await Tokens.GenerateJwt(jwtFactory.GenerateClaimsIdentity(identity.UserName, identity.Id), jwtFactory, identity.UserName, jwtOptions, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });
            return jwt;
        }
    }
}
