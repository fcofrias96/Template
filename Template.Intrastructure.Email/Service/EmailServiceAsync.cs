using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.DTOs.Email;
using Template.Infrastructure.Email.Settings;

namespace Template.Infrastructure.Email.Service
{
    public class EmailServiceAsync : IEmailServiceAsync
    {
        readonly private MailSettings _mailSetttings;

        public EmailServiceAsync(IOptions<MailSettings> option)
        {
            _mailSetttings = option.Value;
        }

        public async Task SendAsync(EmailDTO emailSendDTO)
        {

            BodyBuilder body = new() { HtmlBody = emailSendDTO.Body };
            MimeMessage email = new()
            {
                Sender = MailboxAddress.Parse(_mailSetttings.DisplayName + " <" + _mailSetttings.EmailFrom + ">"),
                Subject = emailSendDTO.Subject,
                Body = body.ToMessageBody()
            };

            email.To.Add(MailboxAddress.Parse(emailSendDTO.To));

            using (SmtpClient smtp = new())
            {
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtp.Connect(_mailSetttings.SmtpHost, _mailSetttings.SmtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSetttings.SmtpUser, _mailSetttings.SmtpPass);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);

            }
        }



    }
}

