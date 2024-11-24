using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Template.Application.Constants;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Models.DTOs.V1.Auth;
using Template.Application.Models.DTOs.V1.Emails;
using Template.Application.Models.DTOs.V1.Token;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;
using Template.Security.Seeds;
using Template.Security.Settings;

namespace Template.Security.Services.V1
{
    public class AuthServiceAsync : IAuthServiceAsync
    {
        private readonly IEmailServiceAsync _emailServiceAsync;
        private readonly IHttpContextAccessor _httpContext;
        private readonly JwtSettings _jwtSettings;
        private readonly SignInManager<User> _singInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserServiceAsync _userServiceAsync;
        private IReadOnlyList<string> _userRoles;

        public AuthServiceAsync(
            IEmailServiceAsync emailServiceAsync,
            IOptions<JwtSettings> jwtSettings,
            UserManager<User> userManager,
            SignInManager<User> singInManager,
            IHttpContextAccessor httpContext,
            IUserServiceAsync userServiceAsync
            )
        {
            _emailServiceAsync = emailServiceAsync;
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _singInManager = singInManager;
            _httpContext = httpContext;
            _userServiceAsync = userServiceAsync;
        }

        public async Task<ApiResponse> ChangeEmailAsync(ChangeEmailDto changeEmailDto)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId != changeEmailDto.UserId.ToString())
            {
                throw new ForbiddenException();
            }

            var user = await FindUserByIdAsync(changeEmailDto.UserId.ToString());
            var passwordValid = await _userManager.CheckPasswordAsync(user, changeEmailDto.CurrentPassword);
            if (!passwordValid)
            {
                throw new BadRequestException("Incorrect password.");
            }
            if (user.Email == changeEmailDto.NewEmail)
            {
                throw new BadRequestException("New email address is the same as the current email address.");
            }

            var changeEmailLink = await GenerateChangeEmailLinkAsync(user, changeEmailDto.NewEmail);
            var emailDto = CreateChangeEmailNotification(user, changeEmailDto.NewEmail, changeEmailLink);
            await _emailServiceAsync.SendEmailAsync(emailDto);

            return new ApiResponse("A confirmation email has been sent to your new email address. Please check your email to confirm the change.");
        }

        public async Task<ApiResponse> ConfirmChangeEmailAsync(ConfirmChangeEmailDto confirmChangeEmailDto)
        {
            var user = await FindUserByIdAsync(confirmChangeEmailDto.UserId.ToString());

            if (user.Email == confirmChangeEmailDto.NewEmail)
            {
                throw new BadRequestException("New email address is the same as the current email address.");
            }

            var token = Encoding.UTF8.GetString(Convert.FromBase64String(confirmChangeEmailDto.Token));
            var result = await _userManager.ChangeEmailAsync(user, confirmChangeEmailDto.NewEmail, token);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Error changing email.");
            }

            await RevokeTokenAsync(user.Id);
            return new ApiResponse("Your email address has been successfully changed.");
        }

        public async Task<ApiResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await FindUserByIdAsync(confirmEmailDto.UserId);

            if (user.EmailConfirmed)
            {
                return new ApiResponse("Email already confirmed.");
            }

            var token = Encoding.UTF8.GetString(Convert.FromBase64String(confirmEmailDto.Token));
            await _userManager.ConfirmEmailAsync(user, token);
            return new ApiResponse($"Account Confirmed for {user.Email}.");
        }

        public async Task<ApiResponse> CreateAsync(UserCreateDto userCreateDto)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(userCreateDto.UserName);
            if (userWithSameUserName != null)
            {
                throw new BadRequestException($"The username '{userCreateDto.UserName}' is already taken. Please choose a different username.");
            }

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

            var result = await _userManager.CreateAsync(user, userCreateDto.Password);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"User creation failed: {errorMessage}");
            }

            await _userManager.AddToRoleAsync(user, Roles.User);

            var itemsModuleId = ResourceSeed.Data.First(m => m.Name == "Items").Id;
            //var readPermissionId = PermissionSeed.Data.First(p => p.ActionId == Actions.Read).Id;

            //var modulePermission = new List<ModulePermissionAddDTO>
            //    {
            //        new ModulePermissionAddDTO
            //        {
            //            ModuleId = itemsModuleId,
            //            PermissionIds = new List<int> { readPermissionId }
            //        }
            //    };

            //await _userServiceAsync.AddModulesAsync(Guid.Parse(user.Id), modulePermission);

            var emailVerificationLink = await GenerateEmailVerificationLinkAsync(user);

            var emailDto = CreateConfirmationEmail(user, emailVerificationLink);
            await _emailServiceAsync.SendEmailAsync(emailDto);

            var message = $"User registered successfully. A confirmation email has been sent to {user.Email}. Please confirm your account by visiting the URL provided in the email.";
            return new ApiResponse(message);
        }

        public async Task<ApiResponse> ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    throw new BadRequestException($"Email for user '{model.Email}' is not confirmed.");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                var baseUrl = $"{_jwtSettings.Audience}/reset-password";
                var resetPasswordUrl = QueryHelpers.AddQueryString(baseUrl, "token", encodedToken);
                resetPasswordUrl = QueryHelpers.AddQueryString(resetPasswordUrl, "userId", user.Id);

                var tokenDuration = _jwtSettings.PasswordResetTokenDurationInHours;
                string duration = $"{tokenDuration} hour{(tokenDuration >= 2 ? "s" : "")}";

                var emailRequest = new EmailDto
                {
                    Body = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                            <h2>Reset Your Password</h2>
                            <p>Dear {user.FirstName} {user.LastName},</p>
                            <p>You requested a password reset.</p>
                            <p>Please click the following link to reset your password:</p>
                            <p><a href='{resetPasswordUrl}'>Reset Password</a></p>
                            <p>This link is valid for {duration}.</p>
                            <p>If you did not request this, please ignore this email.</p>
                            <p>Best regards,<br>Your Company Team</p>
                        </body>
                        </html>",
                    To = model.Email,
                    Subject = "Reset Password",
                };
                await _emailServiceAsync.SendEmailAsync(emailRequest);
            }

            return new ApiResponse("If an account with that email exists, a password reset link has been sent. Please check your email.");
        }

        public async Task<ApiResponse> LogoutAsync()
        {
            var userId = GetCurrentUserId();
            await RevokeTokenAsync(userId);
            return new ApiResponse("Logged out successfully");
        }

        public async Task<ApiResponse<AuthenticationResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await FindUserByEmailAsync(loginDto.Email);

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

            var result = await _singInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: true, lockoutOnFailure: true); ;

            if (result.Succeeded)
            {
                var response = new AuthenticationResponseDto();
                JwtSecurityToken jwtSecurityToken = await GenerateJWTokenAsync(user);
                var refreshToken = GenerateRefreshToken();
                response.Id = user.Id;
                response.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                response.Email = user.Email;
                response.UserName = user.UserName;
                response.Roles = _userRoles;
                response.RefreshToken = refreshToken;

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
                user.IsTokenRevoked = false;
                user.TokenRevokedTime = null;
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

        public async Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ForbiddenException("No refresh token provided");
            }

            var user = await _userServiceAsync.GetUserByRefreshToken(refreshToken);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow || user.IsTokenRevoked)
            {
                throw new ForbiddenException("Invalid refresh token");
            }

            var newJwtToken = await GenerateJWTokenAsync(user);
            var newRefreshToken = user.RefreshToken;
            var timeToTokenExpiry = (user.RefreshTokenExpiryTime - DateTime.UtcNow).TotalDays;

            if (timeToTokenExpiry < 1)
            {
                newRefreshToken = GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationInDays);
            }

            await _userManager.UpdateAsync(user);

            var response = new RefreshTokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newJwtToken),
                RefreshToken = newRefreshToken
            };

            return new ApiResponse<RefreshTokenResponseDto>(response);
        }

        public async Task<ApiResponse> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(resendConfirmationEmailDto.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                {
                    throw new BadRequestException("Email is already confirmed.");
                }

                var emailVerificationLink = await GenerateEmailVerificationLinkAsync(user);
                var emailDto = CreateConfirmationEmail(user, emailVerificationLink);

                await _emailServiceAsync.SendEmailAsync(emailDto);
            }
            return new ApiResponse("If an account with that email exists, a confirmation email has been resent. Please check your email.");
        }

        public async Task<ApiResponse> ResetPassword(ResetPasswordDto model)
        {
            var user = await FindUserByIdAsync(model.UserId.ToString());
            var token = Encoding.UTF8.GetString(Convert.FromBase64String(model.Token));
            await _userManager.ResetPasswordAsync(user, token, model.Password);
            await RevokeTokenAsync(user.Id);
            return new ApiResponse($"Password Reset Successful for {user.UserName}.");
        }

        public async Task RevokeTokenAsync(string userId)
        {
            var user = await FindUserByIdAsync(userId);

            user.IsTokenRevoked = true;
            user.TokenRevokedTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        private EmailDto CreateChangeEmailNotification(User user, string newEmail, string changeEmailLink)
        {
            var tokenDuration = _jwtSettings.ChangeEmailTokenDurationInHours;
            string duration = $"{tokenDuration} hour{(tokenDuration >= 2 ? "s" : "")}";

            return new EmailDto
            {
                To = newEmail,
                Subject = "Confirm your new email address",
                Body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <h2>Confirm Your Email Address</h2>
                    <p>Dear {user.FirstName} {user.LastName},</p>
                    <p>You have requested to update your email address.</p>
                    <p>Please confirm your new email address by clicking the link below:</p>
                    <p><a href='{changeEmailLink}'>Confirm Email</a></p>
                    <p>This link is valid for {duration}.</p>
                    <p>If you did not request this change, please ignore this email or contact support.</p>
                    <p>Best regards,<br>Your Company Team</p>
                </body>
                </html>"
            };
        }

        private EmailDto CreateConfirmationEmail(User user, string emailConfirmationLink)
        {
            var tokenDuration = _jwtSettings.EmailConfirmationTokenDurationInHours;
            string duration = $"{tokenDuration} hour{(tokenDuration >= 2 ? "s" : "")}";

            return new EmailDto
            {
                To = user.Email,
                Body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <h2>Confirm Your Account</h2>
                <p>Dear {user.FirstName} {user.LastName},</p>
                <p>Thank you for registering. Please confirm your account by clicking the link below:</p>
                <p><a href='{emailConfirmationLink}' style='color: #1a73e8;'>Confirm Registration</a></p>
                <p>This link is valid for {duration}.</p>
                <p>If you did not create an account, please ignore this email.</p>
                <p>Best regards,<br>Your Company Team</p>
            </body>
            </html>",
                Subject = "Confirm Registration"
            };
        }

        private async Task<User> FindUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new BadRequestException("The email or password is incorrect. Please try again.");
            }

            return user;
        }

        private async Task<User> FindUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                throw new NotFoundException("User", userId);
            }

            return user;
        }

        private async Task<string> GenerateChangeEmailLinkAsync(User user, string newEmail)
        {
            var changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            var encodedChangeEmailToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(changeEmailToken));
            var httpRequest = _httpContext?.HttpContext?.Request;
            var currentRequestUrl = httpRequest != null ? httpRequest.GetEncodedUrl() : string.Empty;

            var apiBaseUrl = currentRequestUrl.Substring(0, currentRequestUrl.LastIndexOf("/auth", StringComparison.OrdinalIgnoreCase) + 5);
            var changeEmailUrl = $"{apiBaseUrl}/confirm-change-email";

            var changeEmailLink = QueryHelpers.AddQueryString(changeEmailUrl, "userId", user.Id);
            changeEmailLink = QueryHelpers.AddQueryString(changeEmailLink, "token", encodedChangeEmailToken);
            changeEmailLink = QueryHelpers.AddQueryString(changeEmailLink, "newEmail", newEmail);

            return changeEmailLink;
        }

        private async Task<string> GenerateEmailVerificationLinkAsync(User user)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailConfirmationToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailConfirmationToken));
            var httpRequest = _httpContext?.HttpContext?.Request;
            var currentRequestUrl = httpRequest != null ? httpRequest.GetEncodedUrl() : string.Empty;

            var apiBaseUrl = currentRequestUrl.Substring(0, currentRequestUrl.LastIndexOf("/auth", StringComparison.OrdinalIgnoreCase) + 5);
            var emailConfirmationUrl = $"{apiBaseUrl}/confirm-email";

            var emailVerificationLink = QueryHelpers.AddQueryString(emailConfirmationUrl, "userId", user.Id);
            emailVerificationLink = QueryHelpers.AddQueryString(emailVerificationLink, "token", encodedEmailConfirmationToken);

            return emailVerificationLink;
        }

        private async Task<JwtSecurityToken> GenerateJWTokenAsync(User user)
        {
            var userClaimList = await _userManager.GetClaimsAsync(user);
            _userRoles = (IReadOnlyList<string>)await _userManager.GetRolesAsync(user);
            var roles = new List<Claim>();

            foreach (var role in _userRoles)
            {
                roles.Add(new Claim("roles", role));
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id)
            };

            claims.AddRange(userClaimList);
            claims.AddRange(roles);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.LoginTokenDurationInMinutes),
            signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GetCurrentUserId()
        {
            return _httpContext.HttpContext?.User?.FindFirst("userId")?.Value ?? string.Empty;
        }
    }
}