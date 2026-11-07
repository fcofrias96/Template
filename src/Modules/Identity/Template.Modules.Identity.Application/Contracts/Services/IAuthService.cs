using Template.Modules.Identity.Application.Dtos.Auth;
using Template.Modules.Identity.Application.Dtos.Token;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.SharedKernel.Results;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<Result<AuthenticationResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);

        Task<Result<ApiResponse>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto, CancellationToken cancellationToken);

        Task<Result<ApiResponse>> ForgotPasswordAsync(ForgotPasswordDto model, CancellationToken cancellationToken);

        Task<Result<ApiResponse>> ResetPasswordAsync(ResetPasswordDto model, CancellationToken cancellationToken);

        Task<Result<ApiResponse>> CreateAsync(UserCreateDto userCreateDTO, CancellationToken cancellationToken);

        Task<Result<ApiResponse>> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto, CancellationToken cancellationToken);

        Task<Result<ApiResponse>> ChangeEmailAsync(ChangeEmailDto changeEmailDto, CancellationToken cancellationToken);

        Task<Result<ApiResponse>> ConfirmChangeEmailAsync(ConfirmChangeEmailDto confirmChangeEmailDto, CancellationToken cancellationToken);

        Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken);

        Task<Result<ApiResponse>> LogoutAsync(CancellationToken cancellationToken);
    }
}
