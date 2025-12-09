using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Template.Application.Constants;
using Template.Application.Exceptions;
using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.Token;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;
using Template.Security.Settings;

namespace Template.Security.Services.V1
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userServiceAsync;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuthLinkService _authLinkService;
        private readonly IAuthEmailService _authEmailService;
        private readonly ICurrentUserService _currentUserService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IOptions<JwtSettings> jwtSettings,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserService userServiceAsync,
            IJwtTokenService jwtTokenService,
            IAuthLinkService authLinkService,
            IAuthEmailService authEmailService,
            ICurrentUserService currentUserService)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _userServiceAsync = userServiceAsync;
            _jwtTokenService = jwtTokenService;
            _authLinkService = authLinkService;
            _authEmailService = authEmailService;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse> ChangeEmailAsync(ChangeEmailDto changeEmailDto, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            if (currentUserId != changeEmailDto.UserId.ToString())
            {
                throw new ForbiddenException();
            }

            var user = await FindUserByIdAsync(changeEmailDto.UserId.ToString(), cancellationToken);

            var passwordValid = await _userManager.CheckPasswordAsync(user, changeEmailDto.CurrentPassword);
            if (!passwordValid)
            {
                throw new BadRequestException("Incorrect password.");
            }

            if (user.Email == changeEmailDto.NewEmail)
            {
                throw new BadRequestException("New email address is the same as the current email address.");
            }

            var changeEmailLink = await _authLinkService.GenerateChangeEmailLinkAsync(user, changeEmailDto.NewEmail, cancellationToken);
            await _authEmailService.SendChangeEmailNotificationAsync(user, changeEmailDto.NewEmail, changeEmailLink, cancellationToken);

            return new ApiResponse("A confirmation email has been sent to your new email address. Please check your email to confirm the change.");
        }

        public async Task<ApiResponse> ConfirmChangeEmailAsync(ConfirmChangeEmailDto confirmChangeEmailDto, CancellationToken cancellationToken)
        {
            var user = await FindUserByIdAsync(confirmChangeEmailDto.UserId.ToString(), cancellationToken);

            if (user.Email == confirmChangeEmailDto.NewEmail)
            {
                throw new BadRequestException("New email address is the same as the current email address.");
            }

            var token = Encoding.UTF8.GetString(Convert.FromBase64String(confirmChangeEmailDto.Token));

            cancellationToken.ThrowIfCancellationRequested();
            var result = await _userManager.ChangeEmailAsync(user, confirmChangeEmailDto.NewEmail, token);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Error changing email.");
            }

            await RevokeTokenAsync(user.Id, cancellationToken);
            return new ApiResponse("Your email address has been successfully changed.");
        }

        public async Task<ApiResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto, CancellationToken cancellationToken)
        {
            var user = await FindUserByIdAsync(confirmEmailDto.UserId, cancellationToken);

            if (user.EmailConfirmed)
            {
                return new ApiResponse("Email already confirmed.");
            }

            var token = Encoding.UTF8.GetString(Convert.FromBase64String(confirmEmailDto.Token));

            cancellationToken.ThrowIfCancellationRequested();
            await _userManager.ConfirmEmailAsync(user, token);

            return new ApiResponse($"Account Confirmed for {user.Email}.");
        }

        public async Task<ApiResponse> CreateAsync(UserCreateDto userCreateDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userWithSameUserName = await _userManager.FindByNameAsync(userCreateDto.UserName);
            if (userWithSameUserName != null)
            {
                throw new BadRequestException($"The username '{userCreateDto.UserName}' is already taken. Please choose a different username.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            var userWithSameEmail = await _userManager.FindByEmailAsync(userCreateDto.Email);
            if (userWithSameEmail != null)
            {
                throw new BadRequestException($"The email address '{userCreateDto.Email}' is already registered. Please use a different email address.");
            }

            var user = new User
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
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"User creation failed: {errorMessage}");
            }

            await _userManager.AddToRoleAsync(user, Roles.User);

            var confirmationLink = await _authLinkService.GenerateEmailConfirmationLinkAsync(user, cancellationToken);
            await _authEmailService.SendConfirmationEmailAsync(user, confirmationLink, cancellationToken);

            var message = $"User registered successfully. A confirmation email has been sent to {user.Email}. Please confirm your account by visiting the URL provided in the email.";
            return new ApiResponse(message);
        }

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordDto model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    throw new BadRequestException($"Email for user '{model.Email}' is not confirmed.");
                }

                cancellationToken.ThrowIfCancellationRequested();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

                var resetPasswordUrl = await _authLinkService.GenerateResetPasswordLinkAsync(user, encodedToken);

                await _authEmailService.SendForgotPasswordEmailAsync(user, resetPasswordUrl, cancellationToken);
            }

            return new ApiResponse("If an account with that email exists, a password reset link has been sent. Please check your email.");
        }

        public async Task<ApiResponse> LogoutAsync(CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? string.Empty;
            await RevokeTokenAsync(userId, cancellationToken);
            return new ApiResponse("Logged out successfully");
        }

        public async Task<ApiResponse<AuthenticationResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await FindUserByEmailAsync(loginDto.Email, cancellationToken);

            if (!user.EmailConfirmed)
            {
                throw new BadRequestException($"Email for user '{loginDto.Email}' is not confirmed.");
            }

            if (user.LockoutEnabled)
            {
                throw new BadRequestException($"User account '{loginDto.Email}' is locked.");
            }

            if (user.IsDeleted)
            {
                throw new BadRequestException($"User account '{loginDto.Email}' is inactivated.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: true, lockoutOnFailure: true);

            if (result.Succeeded)
            {
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

                return new ApiResponse<AuthenticationResponseDto>(response);
            }
            else if (result.IsLockedOut)
            {
                throw new BadRequestException($"User account '{loginDto.Email}' locked out.");
            }
            else if (result.IsNotAllowed)
            {
                throw new BadRequestException($"Login not allowed for user '{loginDto.Email}'.");
            }
            else
            {
                throw new BadRequestException($"Invalid login attempt for user '{loginDto.Email}'.");
            }
        }

        public async Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(string? refreshToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ForbiddenException("No refresh token provided");
            }

            var user = await _userServiceAsync.GetUserByRefreshToken(refreshToken, cancellationToken);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow || user.IsTokenRevoked)
            {
                throw new ForbiddenException("Invalid refresh token");
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

            var response = new RefreshTokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                RefreshToken = newRefreshToken
            };

            return new ApiResponse<RefreshTokenResponseDto>(response);
        }

        public async Task<ApiResponse> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByEmailAsync(resendConfirmationEmailDto.Email);

            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    throw new BadRequestException("Email is already confirmed.");
                }

                var confirmationLink = await _authLinkService.GenerateEmailConfirmationLinkAsync(user, cancellationToken);
                await _authEmailService.SendConfirmationEmailAsync(user, confirmationLink, cancellationToken);
            }

            return new ApiResponse("If an account with that email exists, a confirmation email has been resent. Please check your email.");
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordDto model, CancellationToken cancellationToken)
        {
            var user = await FindUserByIdAsync(model.UserId.ToString(), cancellationToken);
            var token = Encoding.UTF8.GetString(Convert.FromBase64String(model.Token));

            cancellationToken.ThrowIfCancellationRequested();
            await _userManager.ResetPasswordAsync(user, token, model.Password);

            await RevokeTokenAsync(user.Id, cancellationToken);

            return new ApiResponse($"Password Reset Successful for {user.UserName}.");
        }

        public async Task RevokeTokenAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await FindUserByIdAsync(userId, cancellationToken);

            user.IsTokenRevoked = true;
            user.TokenRevokedTime = DateTimeOffset.UtcNow;

            cancellationToken.ThrowIfCancellationRequested();
            await _userManager.UpdateAsync(user);
        }

        // ---------- Helpers privados ----------

        private async Task<User> FindUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new BadRequestException("The email or password is incorrect. Please try again.");
            }

            return user;
        }

        private async Task<User> FindUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                throw new NotFoundException("User", userId);
            }

            return user;
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
