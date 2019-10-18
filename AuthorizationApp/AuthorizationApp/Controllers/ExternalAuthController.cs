using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AuthorizationApp.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using AuthorizationApp.Services.Interfaces;
using AuthorizationApp.ViewModels;
using AutoMapper;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthorizationApp.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ExternalAuthController : Controller
    {
        private readonly IJwtTokenService jwtService;
        private readonly IMailSender emailService;
        private readonly IExternalLoginService<AppUser> loginService;
        private readonly IMapper mapper;

        public ExternalAuthController(IJwtTokenService jwtService, 
            IMailSender sender, IExternalLoginService<AppUser> logService, IMapper mapper)
        {
            this.jwtService = jwtService;
            emailService = sender;
            this.loginService = logService;
            this.mapper = mapper;
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
                return new OkObjectResult(await jwtService.GetJwtToken(signedUser));
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

                return new OkObjectResult(await jwtService.GetJwtToken(user));

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

        [HttpGet]
        public async Task<ResultViewModel> Associate([FromBody] AssociateViewModel model) 
        {
            if (!model.associateExistingAccount)
            {
                var user = mapper.Map<AppUser>(model);

                var createUserResult = await loginService.CreateUserWithoutPassword(user);

                if (createUserResult.Succeeded)
                {
                    createUserResult = await loginService.AddLoginAsync(user,
                        new ExternalLoginInfo(null, model.LoginProvider, model.ProviderKey,
                        model.ProviderDisplayName));

                    if (createUserResult.Succeeded)
                    {
                        user.EmailConfirmed = true;
                        await loginService.UpdateUser(user);

                        var token = await jwtService.GetJwtToken(user);

                        return new ResultViewModel
                        {
                            Status = Status.Success,
                            Message = $"{user.UserName} has been created successfully",
                            Data = token
                        };
                    }

                }

                var resultErrors = createUserResult.Errors.Select(e => "error description:" + e.Description);

                return new ResultViewModel
                {
                    Status = Status.Error,
                    Message = "Invalid data",
                    Data = string.Join("", resultErrors)
                };

            }

            var userEntity = await loginService.FindByEmailAsync(model.AssociateEmail);

            if(userEntity != null)
            {
                if (!userEntity.EmailConfirmed)
                {
                    return new ResultViewModel
                    {
                        Status = Status.Error,
                        Message = "Invalid data",
                        Data = $"Associated account {model.AssociateEmail} hasn't been confirmed yet. "
                        + "Confirm the account and try again"               
                    };
                }

                var token = await loginService.GenerateEmailConfirmationTokenAsync(userEntity);

                var callbackUrl = Url.Action("ConfirmExternalProvider", "Account",
                        values: new
                        {
                            userId = userEntity.Id,
                            code = token,
                            loginProvider = model.LoginProvider,
                            providerDisplayName = model.LoginProvider,
                            providerKey = model.ProviderKey
                        },
                        protocol: HttpContext.Request.Scheme);

                await emailService.SendEmailAsync(userEntity.Email, $"Confirm {model.ProviderDisplayName} external login",
                $"Please confirm association of your {model.ProviderDisplayName} " +
                $"account by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>");

                return new ResultViewModel
                {
                    Status = Status.Success,
                    Message = "External account association is pending. Please check your email"
                };
            }

            return new ResultViewModel
            {
                Status = Status.Error,
                Message = "Invalid data",
                Data = $"User with email {model.AssociateEmail} not found"
            };
        }
    }
}
