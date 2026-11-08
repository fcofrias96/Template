using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using Template.Modules.Identity.Contracts.Authorization;
using Template.Modules.Identity.Application.Dtos.Auth;
using Template.Modules.Identity.Application.Dtos.Token;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Errors;
using Template.Modules.Identity.Domain.Entities;
using Template.Modules.Identity.Application.Settings;
using Template.SharedKernel.Results;

namespace Template.Modules.Identity.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userServiceAsync;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuthLinkService _authLinkService;
        private readonly IAuthEmailService _authEmailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccessTokenRevocationService _accessTokenRevocationService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IOptions<JwtSettings> jwtSettings,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserService userServiceAsync,
            IJwtTokenService jwtTokenService,
            IAuthLinkService authLinkService,
            IAuthEmailService authEmailService,
            ICurrentUserService currentUserService,
            IAccessTokenRevocationService accessTokenRevocationService)
        {
            _accessTokenRevocationService = accessTokenRevocationService;
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _userServiceAsync = userServiceAsync;
            _jwtTokenService = jwtTokenService;
            _authLinkService = authLinkService;
            _authEmailService = authEmailService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ApiResponse>> ChangeEmailAsync(ChangeEmailDto changeEmailDto, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            if (currentUserId != changeEmailDto.UserId)
            {
                return UserErrors.Forbidden;
            }

            var userResult = await FindUserByIdAsync(changeEmailDto.UserId, cancellationToken);
            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            var user = userResult.Value;

            var passwordValid = await _userManager.CheckPasswordAsync(user, changeEmailDto.CurrentPassword);
            if (!passwordValid)
            {
                return AuthErrors.IncorrectPassword;
            }

            if (user.Email == changeEmailDto.NewEmail)
            {
                return AuthErrors.SameEmail;
            }

            var changeEmailLink = await _authLinkService.GenerateChangeEmailLinkAsync(user, changeEmailDto.NewEmail, cancellationToken);
            await _authEmailService.SendChangeEmailNotificationAsync(user, changeEmailDto.NewEmail, changeEmailLink, cancellationToken);

            return new ApiResponse("A confirmation email has been sent to your new email address. Please check your email to confirm the change.");
        }

        public async Task<Result<ApiResponse>> ConfirmChangeEmailAsync(ConfirmChangeEmailDto confirmChangeEmailDto, CancellationToken cancellationToken)
        {
            var userResult = await FindUserByIdAsync(confirmChangeEmailDto.UserId, cancellationToken);
            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            var user = userResult.Value;

            if (user.Email == confirmChangeEmailDto.NewEmail)
            {
                return AuthErrors.SameEmail;
            }

            var tokenResult = DecodeToken(confirmChangeEmailDto.Token);
            if (tokenResult.IsFailure)
            {
                return tokenResult.Error;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var result = await _userManager.ChangeEmailAsync(user, confirmChangeEmailDto.NewEmail, tokenResult.Value);

            if (!result.Succeeded)
            {
                return AuthErrors.ChangeEmailFailed(result);
            }

            await RevokeTokenAsync(user, cancellationToken);
            return new ApiResponse("Your email address has been successfully changed.");
        }

        public async Task<Result<ApiResponse>> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto, CancellationToken cancellationToken)
        {
            var userResult = await FindUserByIdAsync(confirmEmailDto.UserId, cancellationToken);
            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            var user = userResult.Value;

            if (user.EmailConfirmed)
            {
                return new ApiResponse("Email already confirmed.");
            }

            var tokenResult = DecodeToken(confirmEmailDto.Token);
            if (tokenResult.IsFailure)
            {
                return tokenResult.Error;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var result = await _userManager.ConfirmEmailAsync(user, tokenResult.Value);

            if (!result.Succeeded)
            {
                return AuthErrors.EmailConfirmationFailed(result);
            }

            return new ApiResponse($"Account Confirmed for {user.Email}.");
        }

        public async Task<Result<ApiResponse>> CreateAsync(UserCreateDto userCreateDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userWithSameUserName = await _userManager.FindByNameAsync(userCreateDto.UserName);
            if (userWithSameUserName != null)
            {
                return AuthErrors.UserNameTaken(userCreateDto.UserName);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var userWithSameEmail = await _userManager.FindByEmailAsync(userCreateDto.Email);
            if (userWithSameEmail != null)
            {
                return AuthErrors.EmailTaken(userCreateDto.Email);
            }

            var user = new ApplicationUser
            {
                Email = userCreateDto.Email,
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                UserName = userCreateDto.UserName
            };

            cancellationToken.ThrowIfCancellationRequested();
            var result = await _userManager.CreateAsync(user, userCreateDto.Password);
            if (!result.Succeeded)
            {
                return AuthErrors.RegistrationFailed(result);
            }

            await _userManager.AddToRoleAsync(user, Roles.User);

            var confirmationLink = await _authLinkService.GenerateEmailConfirmationLinkAsync(user, cancellationToken);
            await _authEmailService.SendConfirmationEmailAsync(user, confirmationLink, cancellationToken);

            var message = $"User registered successfully. A confirmation email has been sent to {user.Email}. Please confirm your account by visiting the URL provided in the email.";
            return new ApiResponse(message);
        }

        public async Task<Result<ApiResponse>> ForgotPasswordAsync(ForgotPasswordDto model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    return AuthErrors.EmailNotConfirmed(model.Email);
                }

                cancellationToken.ThrowIfCancellationRequested();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

                var resetPasswordUrl = await _authLinkService.GenerateResetPasswordLinkAsync(user, encodedToken);

                await _authEmailService.SendForgotPasswordEmailAsync(user, resetPasswordUrl, cancellationToken);
            }

            return new ApiResponse("If an account with that email exists, a password reset link has been sent. Please check your email.");
        }

        public async Task<Result<ApiResponse>> LogoutAsync(CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is not int userId)
            {
                return AuthErrors.NotAuthenticated;
            }

            var userResult = await FindUserByIdAsync(userId, cancellationToken);
            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            await RevokeTokenAsync(userResult.Value, cancellationToken);
            return new ApiResponse("Logged out successfully");
        }

        public async Task<Result<ApiResponse>> RevokeTokensAsync(int userId, CancellationToken cancellationToken)
        {
            var userResult = await FindUserByIdAsync(userId, cancellationToken);
            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            await RevokeTokenAsync(userResult.Value, cancellationToken);
            return new ApiResponse($"Tokens revoked for user {userId}.");
        }

        public async Task<Result<AuthenticationResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null)
            {
                return AuthErrors.InvalidCredentials;
            }

            if (!user.EmailConfirmed)
            {
                return AuthErrors.EmailNotConfirmed(loginDto.Email);
            }

            if (user.LockoutEnabled)
            {
                return AuthErrors.AccountLocked(loginDto.Email);
            }

            if (user.IsDeleted)
            {
                return AuthErrors.AccountInactive(loginDto.Email);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: true, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                return AuthErrors.LockedOut(loginDto.Email);
            }

            if (result.IsNotAllowed)
            {
                return AuthErrors.LoginNotAllowed(loginDto.Email);
            }

            if (!result.Succeeded)
            {
                return AuthErrors.InvalidLoginAttempt(loginDto.Email);
            }

            var (jwtSecurityToken, roles) = await _jwtTokenService.GenerateTokenAsync(user, cancellationToken);
            var refreshToken = GenerateRefreshToken();

            var response = new AuthenticationResponseDto
            {
                Id = user.Id,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles,
                RefreshToken = refreshToken,
                Resources = await _userServiceAsync.GetResourcesAsync(user.Id, cancellationToken)
            };

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
            user.IsTokenRevoked = false;
            user.TokenRevokedTime = null;

            cancellationToken.ThrowIfCancellationRequested();
            await _userManager.UpdateAsync(user);

            return response;
        }

        public async Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return AuthErrors.RefreshTokenMissing;
            }

            var user = await _userServiceAsync.GetByRefreshToken(refreshToken, cancellationToken);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow || user.IsTokenRevoked)
            {
                return AuthErrors.RefreshTokenInvalid;
            }

            var (newJwtToken, _) = await _jwtTokenService.GenerateTokenAsync(user, cancellationToken);
            var newRefreshToken = user.RefreshToken;
            var timeToTokenExpiry = (user.RefreshTokenExpiryTime - DateTimeOffset.UtcNow).TotalDays;

            if (timeToTokenExpiry < 1)
            {
                newRefreshToken = GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
            }

            cancellationToken.ThrowIfCancellationRequested();
            await _userManager.UpdateAsync(user);

            return new RefreshTokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                RefreshToken = newRefreshToken
            };
        }

        public async Task<Result<ApiResponse>> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByEmailAsync(resendConfirmationEmailDto.Email);

            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    return AuthErrors.EmailAlreadyConfirmed;
                }

                var confirmationLink = await _authLinkService.GenerateEmailConfirmationLinkAsync(user, cancellationToken);
                await _authEmailService.SendConfirmationEmailAsync(user, confirmationLink, cancellationToken);
            }

            return new ApiResponse("If an account with that email exists, a confirmation email has been resent. Please check your email.");
        }

        public async Task<Result<ApiResponse>> ResetPasswordAsync(ResetPasswordDto model, CancellationToken cancellationToken)
        {
            var userResult = await FindUserByIdAsync(model.UserId, cancellationToken);
            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            var user = userResult.Value;

            var tokenResult = DecodeToken(model.Token);
            if (tokenResult.IsFailure)
            {
                return tokenResult.Error;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var result = await _userManager.ResetPasswordAsync(user, tokenResult.Value, model.Password);

            if (!result.Succeeded)
            {
                return AuthErrors.PasswordResetFailed(result);
            }

            await RevokeTokenAsync(user, cancellationToken);

            return new ApiResponse($"Password Reset Successful for {user.UserName}.");
        }

        // ---------- Helpers privados ----------

        /// <summary>
        /// Revokes the refresh token (persisted) and every access token issued so far (cached until they expire).
        /// </summary>
        private async Task RevokeTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            user.IsTokenRevoked = true;
            user.TokenRevokedTime = DateTimeOffset.UtcNow;

            cancellationToken.ThrowIfCancellationRequested();
            await _userManager.UpdateAsync(user);

            _accessTokenRevocationService.Revoke(user.Id);
        }

        private async Task<Result<ApplicationUser>> FindUserByIdAsync(int userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return UserErrors.NotFound(userId);
            }

            return user;
        }

        /// <summary>
        /// Tokens travel Base64-encoded in links; a malformed one is a client error, not a server fault.
        /// </summary>
        private static Result<string> DecodeToken(string? encodedToken)
        {
            if (string.IsNullOrWhiteSpace(encodedToken))
            {
                return AuthErrors.InvalidToken;
            }

            var buffer = new byte[(encodedToken.Length + 3) / 4 * 3];
            if (!Convert.TryFromBase64String(encodedToken, buffer, out var bytesWritten))
            {
                return AuthErrors.InvalidToken;
            }

            return Encoding.UTF8.GetString(buffer, 0, bytesWritten);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
