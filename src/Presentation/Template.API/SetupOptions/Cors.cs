using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Template.Api.SetupOptions
{
    public static class Cors
    {
        public static Action<CorsOptions> GetOptions(IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
            if (allowedOrigins is null)
                throw new ArgumentNullException(nameof(configuration), "Allowed origins are not configured in the appsettings");

            return options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
            };
        }
    }
}
