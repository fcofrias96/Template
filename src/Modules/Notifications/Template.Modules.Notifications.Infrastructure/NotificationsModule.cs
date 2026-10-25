using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Modules.Notifications.Contracts;
using Template.Modules.Notifications.Infrastructure.Services;
using Template.Modules.Notifications.Infrastructure.Settings;

namespace Template.Modules.Notifications.Infrastructure
{
    /// <summary>
    /// Composition root of the Notifications module (outgoing email).
    /// </summary>
    public static class NotificationsModule
    {
        public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
