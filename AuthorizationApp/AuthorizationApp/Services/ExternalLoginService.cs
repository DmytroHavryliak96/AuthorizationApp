using AuthorizationApp.Models;
using AuthorizationApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.Services
{
    // realy must be implemented?

    public class ExternalLoginService : IExternalLoginService<AppUser>
    {
        private readonly ApplicationContext db;
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        public ExternalLoginService(ApplicationContext db, SignInManager<AppUser> signIn, UserManager<AppUser> userManager)
        {
            this.db = db;
            signInManager = signIn;
            this.userManager = userManager;
        }

        public async Task<IdentityResult> AddLoginAsync(AppUser user, UserLoginInfo login)
        {
            return await userManager.AddLoginAsync(user, login);
        }

        public async Task<bool> CheckExternalProviderSignIn(string loginProvider, string providerKey)
        {
            var result = await GetIdentityLogin(loginProvider, providerKey);

            if (result == null)
                return false;

            var id = result.UserId;

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return false;

            if(!user.EmailConfirmed)
                return false;

            return true;
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            return signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId);
        }

        public async Task<IdentityResult> CreateUserWithoutPassword(AppUser identity)
        {
            return await userManager.CreateAsync(identity);
        }

        public async Task<AppUser> FindByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<AppUser> FindByProviderAsync(string loginProvider, string providerKey)
        {
            var result = await GetIdentityLogin(loginProvider, providerKey);

            if (result == null)
                return null;

            var res = await userManager.FindByIdAsync(result.UserId);
            return res;
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(AppUser user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            return await signInManager.GetExternalLoginInfoAsync();
        }

        public async Task<IdentityResult> UpdateUser(AppUser identity)
        {
            return await userManager.UpdateAsync(identity);
        }

        private async Task<IdentityUserLogin<string>> GetIdentityLogin(string loginProvider, string providerKey)
        {
            return await Task.Run(() => db.IdentityUserLogins.Where(login => login.LoginProvider.Equals(loginProvider)
                                                   && login.ProviderKey.Equals(providerKey)).FirstOrDefault());
        }
    }
}
