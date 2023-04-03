using Template.Core.Application.DTOs.Email;

namespace Template.Core.Application.Contracts.Services
{
    public interface IEmailServiceAsync
    {
        Task SendAsync(EmailDTO emailDTO);
    }
}