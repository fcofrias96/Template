using Template.Core.Application.DTOs.Auth;

namespace Template.Infrastructure.Identity.Contracts.Services
{
    public interface IAuthServiceAsync
    {
        Task<AuthenticationResponseDTO> LoginAsync(LoginDTO request, string ipAddress);

        Task<string> ConfirmEmailAsync(ConfirmEmailDTO confirmEmailDTO);

        Task ForgotPassword(ForgotPasswordDTO model);

        Task<string> ResetPassword(ResetPasswordDTO model);
    }
}