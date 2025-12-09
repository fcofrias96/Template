using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.Token;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;

namespace Template.Security.Contracts.Services.V1
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthenticationResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);

        Task<ApiResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto, CancellationToken cancellationToken);

        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto model, CancellationToken cancellationToken);

        Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto model, CancellationToken cancellationToken);

        Task<ApiResponse> CreateAsync(UserCreateDto userCreateDTO, CancellationToken cancellationToken);

        Task<ApiResponse> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto, CancellationToken cancellationToken);

        Task<ApiResponse> ChangeEmailAsync(ChangeEmailDto changeEmailDto, CancellationToken cancellationToken);

        Task<ApiResponse> ConfirmChangeEmailAsync(ConfirmChangeEmailDto confirmChangeEmailDto, CancellationToken cancellationToken);

        Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken);

        Task<ApiResponse> LogoutAsync(CancellationToken cancellationToken);
    }
}