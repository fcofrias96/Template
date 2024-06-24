using Template.Application.Models.DTOs.V1.Email;

namespace Template.Application.Contracts.Services.V1
{
    public interface IEmailServiceAsync
    {
        Task SendEmailAsync(EmailDto emailDto);
    }
}