using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Template.Core.Application.SetupOptions
{
    public static class Cors
    {
        public static Action<CorsOptions> Options = options =>
        {
            options.AddPolicy("AllowAll", policy =>
               policy.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader());
        };
    }
}