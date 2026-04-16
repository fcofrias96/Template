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
            services.AddScoped<IActionRepository, ActionRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();

            // Map IBaseRepository<TEntity> to the concrete repository implementations so BaseServiceDeps<TEntity> resolves correctly
            services.AddScoped<IBaseRepository<ApplicationRole>>(sp => sp.GetRequiredService<IRoleRepository>());
            services.AddScoped<IBaseRepository<ApplicationUser>>(sp => sp.GetRequiredService<IUserRepository>());
            services.AddScoped<IBaseRepository<Permission>>(sp => sp.GetRequiredService<IPermissionRepository>());
            services.AddScoped<IBaseRepository<PermissionAction>>(sp => sp.GetRequiredService<IActionRepository>());
            services.AddScoped<IBaseRepository<Resource>>(sp => sp.GetRequiredService<IResourceRepository>());

            //Services
            services.AddScoped<IActionService, ActionService>();
            services.AddScoped<IAuthEmailService, AuthEmailService>();
            services.AddScoped<IAuthLinkService, AuthLinkService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IMenuCacheInvalidator, MenuCacheInvalidator>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IPermissionCacheInvalidator, PermissionCacheInvalidator>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserAuthCacheInvalidator, UserAuthCacheInvalidator>();
            services.AddScoped<IUserAuthCacheService, UserAuthCacheService>();
            services.AddScoped<IUserService, UserService>();

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
                        string userName = context.HttpContext.GetUserName();

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