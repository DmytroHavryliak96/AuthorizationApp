using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AuthorizationApp.Auth;
using AuthorizationApp.Extensions;
using AuthorizationApp.Helpers;
using System.Text;
using System.Net;
using FluentValidation;
using AuthorizationApp.ViewModels;
using AuthorizationApp.ViewModels.Validations;
using AuthorizationApp.Services;

namespace AuthorizationApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));

            var appSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(appSettings);
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JwtKey));

            services.AddSingleton<IJwtFactory, JwtFactory>();

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            var jwtAppSettingsOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            services.Configure<JwtIssuerOptions>(options => {
                options.Issuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                }
            );

            services.Configure<AuthMessageSenderOptions>(options =>
            {
                options.Password = Configuration["SmtpServer:Password"];
                options.Gmail = Configuration["SmtpServer:Gmail"];
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(congigureOptions =>
           {
               congigureOptions.ClaimsIssuer = jwtAppSettingsOptions[nameof(JwtIssuerOptions.Issuer)];
               congigureOptions.TokenValidationParameters = tokenValidationParameters;
               congigureOptions.SaveToken = true;
           });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => {
                    policy.RequireClaim(Constants.Strings.JwtClaimsIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess);
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    });
            });
            
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            /*services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });*/

            services.AddAutoMapper(typeof(Startup));
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddTransient<IValidator<CredentialViewModel>, CredentialsViewModelValidator>();
            services.AddTransient<IValidator<RegistrationViewModel>, RegistrationViewModelValidator>();
            services.AddTransient<IValidator<ForgotPasswordViewModel>, ForgotPasswordViewModelValidator>();
            services.AddTransient<IValidator<ResetPasswordViewModel>, ResetPasswordViewModelValidator>();

            services.AddSingleton<IMailSender, AuthMessageSender>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(builder =>
            {
                builder.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                            var error = context.Features.Get<IExceptionHandlerFeature>();
                            if (error != null)
                            {
                                context.Response.AddApplicationError(error.Error.Message);
                                await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                            }
                        }
                    );
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();

           /* app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });*/


        }
    }
}
