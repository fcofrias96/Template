using Template.BuildingBlocks.Infrastructure.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Template.BuildingBlocks.Infrastructure.Auditing;
using Template.BuildingBlocks.Infrastructure.Persistence;
using Template.BuildingBlocks.Application.Constants;
using Template.Modules.Identity.Contracts.Authorization;
using Template.BuildingBlocks.Application.Extensions;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Infrastructure;
using Template.Modules.Identity.Domain.Entities;
using Template.Modules.Identity.Infrastructure.Repositories;
using Template.Modules.Identity.Application.Services;
using Template.Modules.Identity.Infrastructure.Services;
using Template.Modules.Identity.Application.Settings;
using Template.Modules.Identity.Infrastructure.TokenProviderConfigurations;
using Template.Modules.Identity.Infrastructure.TokenProviderConfigurations.ChangeEmail;
using Template.Modules.Identity.Infrastructure.TokenProviderConfigurations.EmailConfirmation;
using Template.Modules.Identity.Infrastructure.TokenProviderConfigurations.PasswordReset;

namespace Template.Modules.Identity.Infrastructure
{
    public static class ServiceExtensions
    {
        public const string ModuleName = "Identity";

        /// <summary>
        /// Persistence, ASP.NET Identity, token issuing and JWT authentication of the Identity module.
        /// </summary>
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext

            services.AddModuleDbContext<IdentityModuleDbContext>(configuration, ModuleName, IdentityModuleDbContext.Schema);

            // Users/roles are audited like any other entity, but secrets never reach the audit log.
            services.Configure<AuditOptions>(options => options
                .Mask<ApplicationUser>(
                    nameof(ApplicationUser.PasswordHash),
                    nameof(ApplicationUser.SecurityStamp),
                    nameof(ApplicationUser.ConcurrencyStamp),
                    nameof(ApplicationUser.RefreshToken))
                .Mask<ApplicationRole>(nameof(ApplicationRole.ConcurrencyStamp)));

            #endregion DbContext

            #region Configure

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            #endregion Configure

            #region Identity

            var jwtSettings = new JwtSettings();
            configuration.Bind("JwtSettings", jwtSettings);

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenProviders.EmailConfirmation;
                options.Tokens.PasswordResetTokenProvider = TokenProviders.PasswordReset;
                options.Tokens.ChangeEmailTokenProvider = TokenProviders.ChangeEmail;
            }).AddEntityFrameworkStores<IdentityModuleDbContext>()
              .AddDefaultTokenProviders()
              .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>(TokenProviders.EmailConfirmation)
              .AddTokenProvider<PasswordResetTokenProvider<ApplicationUser>>(TokenProviders.PasswordReset)
              .AddTokenProvider<ChangeEmailTokenProvider<ApplicationUser>>(TokenProviders.ChangeEmail);

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

            services.AddRepositoriesImplementingBaseRepository(typeof(ServiceExtensions).Assembly)
                    .AddScoped<IRolePermissionRepository, RolePermissionRepository>()
                    .AddScoped<IUserRoleRepository, UserRoleRepository>()
                    .AddScoped<IMenuRepository, MenuRepository>();

            //Services

            services.AddScoped<IAuthLinkService, AuthLinkService>()
                    .AddScoped<ICurrentUserService, CurrentUserService>()
                    .AddScoped<IJwtTokenService, JwtTokenService>()
                    .AddScoped<IUserAuthCacheService, UserAuthCacheService>();

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
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = async (context) =>
                    {
                        var statusCode = StatusCodes.Status401Unauthorized;
                        context.Response.StatusCode = statusCode;
                        context.HandleResponse();

                        var problemDetails = new ProblemDetails
                        {
                            Type = ProblemType.Unauthorized,
                            Title = "Unauthorized",
                            Status = statusCode,
                            Detail = "You are not authenticated"
                        };

                        await context.Response.WriteAsJsonAsync(problemDetails);

                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<IdentityModuleDbContext>>();
                        logger.LogWarning(problemDetails.Detail);
                    },
                    OnForbidden = async (context) =>
                    {
                        var statusCode = StatusCodes.Status403Forbidden;
                        context.Response.StatusCode = statusCode;
                        context.Response.ContentType = "application/json";

                        string pathRequest = context.HttpContext.Request.Path;
                        string userName = context.HttpContext.GetUserId();

                        var problemDetails = new ProblemDetails
                        {
                            Type = ProblemType.Forbidden,
                            Title = "Forbidden",
                            Status = statusCode,
                            Detail = "You are not authorized to access this resource"
                        };

                        await context.Response.WriteAsJsonAsync(problemDetails);

                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<IdentityModuleDbContext>>();
                        logger.LogWarning("User {UserName} is not authorized to access {PathRequest}", userName, pathRequest);
                    }
                };
            });

            #endregion Authentication

            return services;
        }
    }
}
