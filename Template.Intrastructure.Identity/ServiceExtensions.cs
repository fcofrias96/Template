using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Template.Infrastructure.Identity.DbContext;
using Template.Infrastructure.Identity.Models;
using Template.Infrastructure.Identity.Services;
using Template.Core.Application.Constants;
using Template.Core.Application.Extensions;
using Template.Core.Application.Interfaces.Services;
using Template.Core.Application.Wrappers;
using Template.Core.Domain.Settings;

namespace Template.Infrastructure.Identity
{
    public static class ServiceExtensions
    {
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityContext>(option =>
                {
                    option.UseInMemoryDatabase("ApplicationDb");
                });
            }
            else
            {
                services.AddDbContext<IdentityContext>(option =>
                {
                    option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        optionAction => optionAction.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
                });
            }

            #endregion DbContext

            #region Configure

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            #endregion Configure

            #region Identity

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(Token.Provider);

            #endregion Identity

            #region Dependency injection

            services.AddScoped<IAccountService, AccountService>();

            #endregion Dependency injection

            #region Authentication

            var key = Encoding.UTF8.GetBytes(configuration["JWTSettings:key"]);
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = configuration["JWTSettings:Audience"],
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                };
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = async (context) =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        Response<string> response;
                        //string userName = context.HttpContext.GetUserName();

                        if (context.AuthenticateFailure == null)
                        {
                            string message = "You Are not Authenticated";
                            response = new Response<string>(message);
                            Log.Warning(message);
                        }
                        else
                        {
                            response = new Response<string>($"{context?.Error}");
                            Log.Warning(context?.Error);
                        }

                        await context.Response.WriteAsJsonAsync(response);
                    },
                    OnForbidden = async (context) =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var response = new Response<string>("You are not authorized to access this resource");
                        await context.Response.WriteAsJsonAsync(response);
                        string pathRequest = context.HttpContext.Request.Path;
                        string userName = context.HttpContext.GetUserName();

                        Log.Warning($"User {userName} are not authorized to access to {pathRequest}");
                    }
                };
            });

            #endregion Authentication
        }
    }
}