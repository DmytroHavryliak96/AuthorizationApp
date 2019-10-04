using AuthorizationApp.Models;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Principal;

namespace AuthorizationApp.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            this.jwtOptions = jwtOptions.Value;
            ThrowIfInvaliOperations(this.jwtOptions);
        }
        public ClaimsIdentity GenerateClaimsIdentity(string userName, string id)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim(Helpers.Constants.Strings.JwtClaimsIdentifiers.Id, id),
                new Claim(Helpers.Constants.Strings.JwtClaimsIdentifiers.Rol,
                    Helpers.Constants.Strings.JwtClaims.ApiAccess)
            });
        }

        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(jwtOptions.IssuedAt).ToString(),
                    ClaimValueTypes.Integer64),
                identity.FindFirst(Helpers.Constants.Strings.JwtClaimsIdentifiers.Rol),
                identity.FindFirst(Helpers.Constants.Strings.JwtClaimsIdentifiers.Id)
            };

            var jwt = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                notBefore: jwtOptions.NotBefore,
                expires: jwtOptions.Expiration,
                signingCredentials: jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;

        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() -
                                  new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds);



        private static void ThrowIfInvaliOperations(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
