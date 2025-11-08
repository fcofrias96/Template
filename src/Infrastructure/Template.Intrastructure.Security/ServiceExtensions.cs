using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Template.Application.Extensions;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;
using Template.Security.Repositories.V1;
using Template.Security.Services.V1;
using Template.Security.Settings;
using Template.Security.TokenProviderConfigurations;
using Template.Security.TokenProviderConfigurations.ChangeEmail;
using Template.Security.TokenProviderConfigurations.EmailConfirmation;
using Template.Security.TokenProviderConfigurations.PasswordReset;

namespace Template.Security
{
    public static class ServiceExtensions
    {
        public static void AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext

            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<SecurityDbContext>(option =>
                {
                    option.UseInMemoryDatabase("Template.Security");
                });
            }
            else
            {
                services.AddDbContext<SecurityDbContext>(option =>
                {
                    option.UseSqlServer(configuration.GetConnectionString("Template.SecurityConnection"),
                        optionBuilder => optionBuilder.MigrationsAssembly(typeof(SecurityDbContext).Assembly.FullName)
                        .EnableRetryOnFailure(
                                maxRetryCount: 3,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null));

                    option.EnableSensitiveDataLogging();
                });
            }

            #endregion DbContext

            #region Configure

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            #endregion Configure

            #region Identity

            var jwtSettings = new JwtSettings();
            configuration.Bind("JwtSettings", jwtSettings);

            services.AddIdentity<User, Role>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenProviders.EmailConfirmation;
                options.Tokens.PasswordResetTokenProvider = TokenProviders.PasswordReset;
                options.Tokens.ChangeEmailTokenProvider = TokenProviders.ChangeEmail;
            }).AddEntityFrameworkStores<SecurityDbContext>()
              .AddDefaultTokenProviders()
              .AddTokenProvider<EmailConfirmationTokenProvider<User>>(TokenProviders.EmailConfirmation)
              .AddTokenProvider<PasswordResetTokenProvider<User>>(TokenProviders.PasswordReset)
              .AddTokenProvider<ChangeEmailTokenProvider<User>>(TokenProviders.ChangeEmail);

            services.Configure<EmailConfirmationTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(jwtSettings.EmailConfirmationTokenDurationInHours);
            });

            services.Configure<PasswordResetTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(jwtSettings.PasswordResetTokenDurationInHours);
            });

            services.Configure<ChangeEmailTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(jwtSettings.ChangeEmailTokenDurationInHours);
            });

            #endregion Identity

            #region Dependency injection

            // Repositories
            services.AddScoped<IActionRepositoryAsync, ActionRepositoryAsync>();
            services.AddScoped<IPermissionRepositoryAsync, PermissionRepositoryAsync>();
            services.AddScoped<IResourceRepositoryAsync, ResourceRepository>();
            services.AddScoped<IRolePermissionRepositoryAsync, RolePermissionRepositoryAsync>();
            services.AddScoped<IRoleRepositoryAsync, RoleRepositoryAsync>();
            services.AddScoped<IUserRepositoryAsync, UserRepositoryAsync>();

            //Services
            services.AddScoped<IActionServiceAsync, ActionServiceAsync>();
            services.AddScoped<IAuthServiceAsync, AuthServiceAsync>();
            services.AddScoped<IPermissionServiceAsync, PermissionServiceAsync>();
            services.AddScoped<IResourceServiceAsync, ResourceService>();
            services.AddScoped<IRolePermissionServiceAsync, RolePermissionServiceAsync>();
            services.AddScoped<IRoleServiceAsync, RoleServiceAsync>();
            services.AddScoped<IUserServiceAsync, UserServiceAsync>();
            services.AddScoped<IMenuServiceAsync, MenuServiceAsync>();
            services.AddScoped<IPermissionService, PermissionService>();

            #endregion Dependency injection

            #region Authentication

            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

            _ = services.AddAuthentication(options =>
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
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,
                };
                //options.Events = new JwtBearerEvents()
                //{
                //    OnChallenge = async (context) =>
                //    {
                //        var statusCode = StatusCodes.Status401Unauthorized;
                //        context.Response.StatusCode = statusCode;
                //        context.HandleResponse();

                //        var response = new ApiResponse(statusCode, "You are not authenticated");

                //        await context.Response.WriteAsJsonAsync(response);
                //        Log.Warning(response.Message);
                //    },
                //    OnForbidden = async (context) =>
                //    {
                //        var statusCode = StatusCodes.Status403Forbidden;
                //        context.Response.StatusCode = statusCode;
                //        context.Response.ContentType = "application/json";

                //        var response = new ApiResponse(statusCode, "You are not authorized to access this resource");

                //        await context.Response.WriteAsJsonAsync(response);
                //        string pathRequest = context.HttpContext.Request.Path;
                //        string userName = context.HttpContext.GetUserName();

                //        Log.Warning($"User {userName} is not authorized to access {pathRequest}");
                //    }
                //};
            });

            #endregion Authentication
        }
    }
}