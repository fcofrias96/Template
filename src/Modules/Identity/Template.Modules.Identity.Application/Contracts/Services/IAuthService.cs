using Template.Modules.Identity.Application.Dtos.Auth;
using Template.Modules.Identity.Application.Dtos.Token;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Application.Models.Wrappers;

namespace Template.Modules.Identity.Application.Contracts.Services
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
