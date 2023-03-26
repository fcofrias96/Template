using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Application.Contracts.Services;
using Template.Infrastructure.Email.Service;
using Template.Infrastructure.Email.Settings;

namespace Template.Infrastructure.Email
{
    public static class ServiceExtensiones
    {
        public static void AddInfrastructureEmail(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            service.AddTransient<IEmailServiceAsync, EmailServiceAsync>();
        }
    }
}
