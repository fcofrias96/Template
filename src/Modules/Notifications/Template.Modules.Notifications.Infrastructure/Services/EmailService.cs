using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Security;
using Template.Modules.Notifications.Contracts;
using Template.Modules.Notifications.Infrastructure.Settings;

namespace Template.Modules.Notifications.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> option)
        {
            _mailSettings = option.Value;
        }

        public async Task SendEmailAsync(EmailDto emailDto, CancellationToken cancellationToken)
        {
            var bodyBuilder = new BodyBuilder { HtmlBody = emailDto.Body };
            var mimeMessage = new MimeMessage
            {
                Subject = emailDto.Subject,
                Body = bodyBuilder.ToMessageBody()
            };

            mimeMessage.To.Add(MailboxAddress.Parse(emailDto.To));
            mimeMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.EmailFrom));

            using var smtpClient = new SmtpClient();
            smtpClient.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                return sslPolicyErrors == SslPolicyErrors.None;
            };

            try
            {
                await smtpClient.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls, cancellationToken);
                await smtpClient.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass, cancellationToken);
                await smtpClient.SendAsync(mimeMessage, cancellationToken);
            }
            finally
            {
                await smtpClient.DisconnectAsync(true, cancellationToken);
            }
        }
    }
}
