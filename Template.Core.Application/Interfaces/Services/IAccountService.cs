using Template.Core.Application.DTOs.Auth;
using Template.Core.Application.DTOs.Token;
using Template.Core.Application.Wrappers;

namespace Template.Core.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<Response<AuthenticationResponse>> LoginAsync(AuthenticationRequest request);

        Task<Response<RefreshTokenResponseDTO?>> VerifyRefreshToken(RefreshTokenRequestDTO request);
    }
}