using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthorizationApp.Helpers;
using AuthorizationApp.Models;
using AuthorizationApp.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using AuthorizationApp.Services;

namespace AuthorizationApp.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ApplicationContext db;
        private readonly UserManager<AppUser> manager;
        private readonly IMapper mapper;
        private readonly IMailSender emailService;

        public AccountController(ApplicationContext db, UserManager<AppUser> manager, IMapper mapper, IMailSender sender)
        {
            this.db = db;
            this.manager = manager;
            this.mapper = mapper;
            emailService = sender;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = mapper.Map<AppUser>(model);

            var result = await manager.CreateAsync(userIdentity, model.Password);

            if (result.Succeeded) 
            {
                var code = await manager.GenerateEmailConfirmationTokenAsync(userIdentity);
                var callbackUrl = Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new { userId = userIdentity.Id, code = code},
                    protocol: HttpContext.Request.Scheme);
                await emailService.SendEmailAsync(model.Email, "Confirm your account", 
                    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
            }
            else 
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            await db.Customers.AddAsync(new Customer { IdentityId = userIdentity.Id, Location = model.Location });
            await db.SaveChangesAsync();
            
            return new OkObjectResult("Account created. To validate your account, please check your mail");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if(userId == null || code == null)
                return BadRequest("Error");
        
            var user = await manager.FindByIdAsync(userId);
            
            if(user == null)
                return BadRequest("Error");

            var result = await manager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
                return new OkObjectResult("Your email address is successfully confirmed.");
            else
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await manager.FindByNameAsync(model.Email);
                if (user == null || !(await manager.IsEmailConfirmedAsync(user)))
                {
                    return BadRequest("Error");
                }

                string code = await manager.GeneratePasswordResetTokenAsync(user);
                
                await emailService.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by using this code: {code}");
            }
            else
                return BadRequest(Errors.AddErrorToModelState("email", "Invalid email", ModelState));

            return new OkObjectResult("Please check your email to reset your password.");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await manager.FindByNameAsync(model.Email);

            if (user == null)
                return BadRequest("Error");

            var result = await manager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return new OkObjectResult("Your password has been reset");
            }
            else 
                return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));
        }

    }
}
