using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Template.Application.Constants;
using Template.Application.Extensions;
using Template.Application.Models.Wrappers;
using Template.Identity.Authorization;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.DbContext.V1;
using Template.Identity.Entities;
using Template.Identity.Repositories.V1;
using Template.Identity.Services.V1;
using Template.Identity.Settings;
using Template.Identity.TokenProviderConfigurations;
using Template.Identity.TokenProviderConfigurations.ChangeEmail;
using Template.Identity.TokenProviderConfigurations.EmailConfirmation;
using Template.Identity.TokenProviderConfigurations.PasswordReset;

namespace Template.Identity
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
                    option.UseInMemoryDatabase("TemplateIdentity");
                });
            }
            else
            {
                services.AddDbContext<IdentityContext>(option =>
                {
                    option.UseSqlServer(configuration.GetConnectionString("TemplateIdentityConnection"),
                        optionBuilder => optionBuilder.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)
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

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = TokenProviders.EmailConfirmation;
                options.Tokens.PasswordResetTokenProvider = TokenProviders.PasswordReset;
                options.Tokens.ChangeEmailTokenProvider = TokenProviders.ChangeEmail;

            }).AddEntityFrameworkStores<IdentityContext>()
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
            services.AddScoped<IUserRepositoryAsync, UserRepositoryAsync>();
            services.AddScoped<IRoleRepositoryAsync, RoleRepositoryAsync>();
            services.AddScoped<IModuleRepository, ModuleRepository>();
            services.AddScoped<IUserModulePermissionRepositoryAsync, UserModulePermissionRepositoryAsync>();

            //Services
            services.AddScoped<IUserServiceAsync, UserServiceAsync>();
            services.AddScoped<IAuthServiceAsync, AuthServiceAsync>();
            services.AddScoped<IRoleServiceAsync, RoleServiceAsync>();
            services.AddScoped<IModuleService, ModuleService>();

            //Others
            services.AddSingleton<IAuthorizationHandler, ModulePermissionHandler>();

            #endregion Dependency injection

            #region Authentication

            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

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

                        var response = new ApiResponse(statusCode, "You are not authenticated");

                        await context.Response.WriteAsJsonAsync(response);
                        Log.Warning(response.Message);
                    },
                    OnForbidden = async (context) =>
                    {
                        var statusCode = StatusCodes.Status403Forbidden;
                        context.Response.StatusCode = statusCode;
                        context.Response.ContentType = "application/json";

                        var response = new ApiResponse(statusCode, "You are not authorized to access this resource");

                        await context.Response.WriteAsJsonAsync(response);
                        string pathRequest = context.HttpContext.Request.Path;
                        string userName = context.HttpContext.GetUserName();

                        Log.Warning($"User {userName} is not authorized to access {pathRequest}");
                    }
                };
            });


            #endregion Authentication

            #region Authorization

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Permissions.Create, policy => policy.Requirements.Add(new ModulePermissionRequirement(Permissions.Create)));
                options.AddPolicy(Permissions.Read, policy => policy.Requirements.Add(new ModulePermissionRequirement(Permissions.Read)));
                options.AddPolicy(Permissions.Update, policy => policy.Requirements.Add(new ModulePermissionRequirement(Permissions.Update)));
                options.AddPolicy(Permissions.Delete, policy => policy.Requirements.Add(new ModulePermissionRequirement(Permissions.Delete)));
                options.AddPolicy(Permissions.Export, policy => policy.Requirements.Add(new ModulePermissionRequirement(Permissions.Export)));

            });

            #endregion Authorization
        }
    }
}