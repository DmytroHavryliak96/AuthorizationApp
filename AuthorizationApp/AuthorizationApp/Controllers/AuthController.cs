using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationApp.Auth;
using AuthorizationApp.Helpers;
using AuthorizationApp.Models;
using AuthorizationApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AuthorizationApp.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private readonly UserManager<AppUser> userManager;
        private readonly IJwtFactory jwtFactory;
        private readonly JwtIssuerOptions jwtOptions;

        public AuthController(UserManager<AppUser> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            this.userManager = userManager;
            this.jwtFactory = jwtFactory;
            this.jwtOptions = jwtOptions.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody] CredentialViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetUser(credentials.UserName, credentials.Password);

            if(user == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login failure", "Invalid username or password", ModelState));
            }

            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(Errors.AddErrorToModelState("login failure", "You haven't confirm your email", ModelState));
            }

            var identity = GetClaimsIdentity(user);

            var jwt = await Tokens.GenerateJwt(identity, jwtFactory, credentials.UserName, jwtOptions, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });

            return new OkObjectResult(jwt);
        }

        private async Task<AppUser> GetUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<AppUser>(null);

            var userToVerify = await userManager.FindByNameAsync(userName);

            if (userToVerify == null)
                return await Task.FromResult<AppUser>(null);

            if( !await userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult<AppUser>(null);
            }

            return await Task.FromResult<AppUser>(userToVerify);
        }

        private ClaimsIdentity GetClaimsIdentity(AppUser user)
        {
            return jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id);
        }
    }
}
