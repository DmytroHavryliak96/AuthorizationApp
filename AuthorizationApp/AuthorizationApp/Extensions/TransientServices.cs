using AuthorizationApp.Models;
using AuthorizationApp.Services;
using AuthorizationApp.Services.Interfaces;
using AuthorizationApp.ViewModels;
using AuthorizationApp.ViewModels.Validations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.Extensions
{
    public static class TransientServices
    {
        public static void ConfigureTransientServices(this IServiceCollection services)
        {
            services.AddTransient<IValidator<CredentialViewModel>, CredentialsViewModelValidator>();
            services.AddTransient<IValidator<RegistrationViewModel>, RegistrationViewModelValidator>();
            services.AddTransient<IValidator<ForgotPasswordViewModel>, ForgotPasswordViewModelValidator>();
            services.AddTransient<IValidator<ResetPasswordViewModel>, ResetPasswordViewModelValidator>();
            services.AddTransient<IValidator<UpdatePasswordViewModel>, UpdatePasswordViewModelValidator>();

            services.AddTransient<IJwtTokenService, JwtTokenService>();

        }
    }
}
