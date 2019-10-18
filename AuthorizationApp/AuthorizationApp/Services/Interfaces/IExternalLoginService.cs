using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.Services.Interfaces
{
    public interface IExternalLoginService<T> where T : IdentityUser
    {
        public Task<bool> CheckExternalProviderSignIn(string loginProvider, string providerKey);

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync();

        public Task<T> FindByEmailAsync(string email);

        public Task<string> GenerateEmailConfirmationTokenAsync(T user);

        public Task<IdentityResult> AddLoginAsync(T user, UserLoginInfo login);

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null);

        public Task<T> FindByProviderAsync(string loginProvider, string providerKey);
    }
}
