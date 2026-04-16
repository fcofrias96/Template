using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Application.Contracts.Services.V1;
using Template.Email.Service.V1;
using Template.Email.Settings;

namespace Template.Email
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructureEmail(this IServiceCollection service, IConfiguration configuration)
        {
            service.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            service.AddTransient<IEmailService, EmailService>();
        }
    }
}