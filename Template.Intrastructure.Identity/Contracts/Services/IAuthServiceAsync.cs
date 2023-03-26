using Template.Core.Application.DTOs.Auth;
using Template.Core.Application.Wrappers;

namespace Template.Infrastructure.Identity.Contracts.Services
{
    public interface IAuthServiceAsync
    {
        Task<Response<AuthenticationResponseDTO>> LoginAsync(LoginDTO request, string ipAddress);
        Task<Response<string>> ConfirmEmailAsync(ConfirmEmailDTO confirmEmailDTO);
        Task ForgotPassword(ForgotPasswordDTO model);
        Task<Response<string>> ResetPassword(ResetPasswordDTO model);
        
    }
}
