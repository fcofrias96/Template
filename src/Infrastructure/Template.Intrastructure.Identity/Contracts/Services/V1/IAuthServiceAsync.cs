using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Identity.Services.V1;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IAuthServiceAsync
    {
        Task<ApiResponse<AuthenticationResponseDTO>> LoginAsync(LoginDto loginDto, string ipAddress);

        Task<ApiResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);

        Task ForgotPassword(ForgotPasswordDto model);

        Task<ApiResponse> ResetPassword(ResetPasswordDto model);


        Task<ApiResponse> CreateAsync(UserCreateDTO userCreateDTO);
        Task<ApiResponse> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto);
    }
}