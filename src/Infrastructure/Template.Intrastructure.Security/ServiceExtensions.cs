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
using Template.Application.Caching.Invalidators;
using Template.Application.Constants;
using Template.Application.Contracts.Caching.Invalidators;
using Template.Application.Contracts.Repositories.Base;
using Template.Application.Extensions;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services;
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
        public static void AddSecurityInfrastructure(this IServiceCollection services, IConfiguration configuration)
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
                    var connectionString = configuration.GetConnectionString("TemplateSecurityConnection") 
                    ?? throw new InvalidOperationException("Connection string 'TemplateSecurityConnection' not found.");

                    option.UseSqlServer(connectionString,
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
            }).AddEntityFrameworkStores<SecurityDbContext>()
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
                    .AddScoped<IUserRoleRepository, UserRoleRepository>();

            //Services

            services.AddServicesImplementingBaseService(typeof(ServiceExtensions).Assembly)            
                    .AddScoped<IAuthEmailService, AuthEmailService>()
                    .AddScoped<IAuthLinkService, AuthLinkService>()
                    .AddScoped<IAuthService, AuthService>()
                    .AddScoped<ICurrentUserService, CurrentUserService>()
                    .AddScoped<IJwtTokenService, JwtTokenService>()
                    .AddScoped<IMenuCacheInvalidator, MenuCacheInvalidator>()
                    .AddScoped<IMenuService, MenuService>()
                    .AddScoped<IPermissionCacheInvalidator, PermissionCacheInvalidator>()
                    .AddScoped<IUserAuthCacheInvalidator, UserAuthCacheInvalidator>()
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

                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<SecurityDbContext>>();
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

                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<SecurityDbContext>>();
                        logger.LogWarning("User {UserName} is not authorized to access {PathRequest}", userName, pathRequest);
                    }
                };
            });

            #endregion Authentication
        }
    }
}