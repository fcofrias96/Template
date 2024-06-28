using Template.Api.Controllers;
using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Identity.Services.V1;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IAuthServiceAsync
    {
        Task<ApiResponse<AuthenticationResponseDTO>> LoginAsync(LoginDto loginDto);

        Task<ApiResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);

        Task<ApiResponse> ForgotPassword(ForgotPasswordDto model);

        Task<ApiResponse> ResetPassword(ResetPasswordDto model);
        Task<ApiResponse> CreateAsync(UserCreateDTO userCreateDTO);
        Task<ApiResponse> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto);
        Task<ApiResponse> ChangeEmailAsync(ChangeEmailDto changeEmailDto);
        Task<ApiResponse> ConfirmChangeEmailAsync(ConfirmChangeEmailDto confirmChangeEmailDto);
    }
}