﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationApp.Auth;
using AuthorizationApp.Extensions;
using AuthorizationApp.Helpers;
using AuthorizationApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AuthorizationApp.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using AuthorizationApp.Services.Interfaces;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthorizationApp.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ExternalAuthController : Controller
    {
        private readonly IJwtFactory jwtFactory;
        private readonly JwtIssuerOptions jwtOptions;
        private readonly IMailSender emailService;
        private readonly IExternalLoginService<AppUser> loginService;

        public ExternalAuthController(IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, 
            IMailSender sender, IExternalLoginService<AppUser> logService)
        {
            this.jwtFactory = jwtFactory;
            this.jwtOptions = jwtOptions.Value;
            emailService = sender;
            this.loginService = logService;
        }

        [HttpGet]
        public IActionResult SignInWithGoogle()
        {
            var authentificationProperties = loginService.ConfigureExternalAuthenticationProperties("Google",
                Url.Action(nameof(HandleExternalLogin)));
            return new ChallengeResult("Google", authentificationProperties);
        }

        [HttpGet]
        public async Task<IActionResult> HandleExternalLogin(string returnUrl = null, string remoteError = null)
        {
            if(remoteError != null)
            {
                return BadRequest($"Error while login via external provider. Message = {remoteError}");
            }

            var info = await loginService.GetExternalLoginInfoAsync();

            var result = await loginService.CheckExternalProviderSignIn(info.LoginProvider, info.ProviderKey);

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (result)
            {
                var signedUser = await loginService.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                return new OkObjectResult(await GetJwtOnSuccess(signedUser));
            }
               
            var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest($"Email scope access is required to add {info.ProviderDisplayName} provider");
            }

            var user = await loginService.FindByEmailAsync(userEmail);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    var token = await loginService.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Action("ConfirmExternalProvider", "Account",
                        values: new
                        {
                            userId = user.Id,
                            code = token,
                            loginProvider = info.LoginProvider,
                            providerDisplayName = info.LoginProvider,
                            providerKey = info.ProviderKey
                        },
                        protocol: HttpContext.Request.Scheme);

                    await emailService.SendEmailAsync(user.Email, $"Confirm {info.ProviderDisplayName} external login",
                    $"Please confirm association of your {info.ProviderDisplayName} " +
                    $"account by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>");

                    return new OkObjectResult($"External account association with {info.ProviderDisplayName} is pending. Please check your email");
                }

                await loginService.AddLoginAsync(user, info);

                return new OkObjectResult(new { 
                   result = await GetJwtOnSuccess(user), 
                   message = "External login was successfully added"
                });

            }

            return new OkObjectResult(new
            {
                Message = "Use this to associate your account with external provider",
                associate = userEmail,
                loginProvider = info.LoginProvider,
                providerDisplayName = info.ProviderDisplayName,
                providerKey = info.ProviderKey
            });
        }

        private async Task<string> GetJwtOnSuccess(AppUser identity)
        {
            var jwt = await Tokens.GenerateJwt(jwtFactory.GenerateClaimsIdentity(identity.UserName, identity.Id), jwtFactory, identity.UserName, jwtOptions, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            });
            return jwt;
        }
    }
}
