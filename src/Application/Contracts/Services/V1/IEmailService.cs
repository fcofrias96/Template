using Template.Application.Models.DTOs.V1.Emails;

namespace Template.Application.Contracts.Services.V1
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto emailDto, CancellationToken cancellationToken);
    }

}