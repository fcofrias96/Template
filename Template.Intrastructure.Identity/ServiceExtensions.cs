using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Diagnostics;
using System.Text;
using Template.Core.Application.Extensions;
using Template.Core.Application.Wrappers;
using Template.Core.Domain.Constants;
using Template.Infrastructure.Identity.Authorization;
using Template.Infrastructure.Identity.Contracts.Repositories;
using Template.Infrastructure.Identity.Contracts.Services;
using Template.Infrastructure.Identity.DbContext;
using Template.Infrastructure.Identity.Entity;
using Template.Infrastructure.Identity.Repositories;
using Template.Infrastructure.Identity.Services;
using Template.Infrastructure.Identity.Settings;

namespace Template.Infrastructure.Identity
{
    public static class ServiceExtensions
    {
        public static string message = "";
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
                    option.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
                        optionAction => optionAction.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
                });
            }

            #endregion DbContext

            #region Configure

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            #endregion Configure

            #region Identity

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<User>>(Token.Provider);

            #endregion Identity

            #region Dependency injection

            services.AddScoped<IUserServiceAsync, UserServiceAsync>();
            services.AddScoped<IAuthServiceAsync, AuthServiceAsync>();
            services.AddScoped<IRoleServiceAsync, RoleServiceAsync>();
            services.AddScoped<IAuthRepositoryAsync, AuthRepositoryAsync>();
            services.AddScoped<IUserRepositoryAsync, UserRepositoryAsync>();
            services.AddScoped<IRoleRepositoryAsync, RoleRepositoryAsync>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            #endregion Dependency injection

            #region Authentication

            var key = Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]);



            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
                        var statusCode = StatusCodes.Status401Unauthorized;
                        context.Response.StatusCode = statusCode;
                        context.HandleResponse();

                        var response = new ApiResponse<Exception>
                        {
                            StatusCode = statusCode,
                            Message = "You are not authenticated"
                        };

                        await context.Response.WriteAsJsonAsync(response);
                        Log.Warning(response.Message);
                    },
                    OnForbidden = async (context) =>
                    {
                        var statusCode = StatusCodes.Status403Forbidden;
                        context.Response.StatusCode = statusCode;
                        context.Response.ContentType = "application/json";

                        var response = new ApiResponse<Exception>
                        {
                            StatusCode = statusCode,
                            Message = "You are not authorized to access this resource"
                        };

                        await context.Response.WriteAsJsonAsync(response);
                        string pathRequest = context.HttpContext.Request.Path;
                        string userName = context.HttpContext.GetUserName();

                        Log.Warning($"User {userName} is not authorized to access {pathRequest}");
                    }/*,
                    OnAuthenticationFailed = async (context) =>
                    {
                        context.NoResult();
                    }*/
                };


            });

            #endregion Authentication

            #region Authorization

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Create", policy => policy.Requirements.Add(new PermissionRequirement(Permission.Create)));
                options.AddPolicy("Update", policy => policy.Requirements.Add(new PermissionRequirement(Permission.Update)));
                options.AddPolicy("Delete", policy => policy.Requirements.Add(new PermissionRequirement(Permission.Delete)));
                options.AddPolicy("Read", policy => policy.Requirements.Add(new PermissionRequirement(Permission.Read)));
            });

            #endregion Authorization
        }


       
    }
}