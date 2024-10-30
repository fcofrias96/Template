using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Template.Application.SetupOptions
{
    public static class Cors
    {
        public static Action<CorsOptions> GetOptions(IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

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