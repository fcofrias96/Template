
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
using Template.Application.Models.DTOs.V1.Email;
using Template.Application.Models.DTOs.V1.ModulePermission;
using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;
using Template.Identity.Helpers;
using Template.Identity.Seeds;
using Template.Identity.Services.V1;
using Template.Identity.Settings;

namespace SimetricaConsulting.Api.Controllers  
{
    public class AuthServiceAsync : IAuthServiceAsync
    {
        private readonly IEmailServiceAsync _emailServiceAsync;
        private readonly IHttpContextAccessor _httpContext;
        private readonly JwtSettings _jwtSettings;
        private readonly SignInManager<User> _singInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserModulePermissionRepositoryAsync _userModulePermissionRepositoryAsync;
        private readonly IUserServiceAsync _userServiceAsync;
        private List<string> _userRoles;
        public AuthServiceAsync(
            IEmailServiceAsync emailServiceAsync,
            IOptions<JwtSettings> jwtSettings,
            IUserModulePermissionRepositoryAsync userModulePermissionRepositoryAsync,
            UserManager<User> userManager,
            SignInManager<User> singInManager,
            IHttpContextAccessor httpContext,
            IUserServiceAsync userServiceAsync
            )
        {
            _emailServiceAsync = emailServiceAsync;
            _jwtSettings = jwtSettings.Value;
            _userRoles = new List<string>();
            _userModulePermissionRepositoryAsync = userModulePermissionRepositoryAsync;
            _userManager = userManager;
            _singInManager = singInManager;
            _httpContext = httpContext;
            _userServiceAsync = userServiceAsync;
        }

        public async Task<ApiResponse> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await FindByIdAsync(confirmEmailDto.UserId);

            if (user.EmailConfirmed)
            {
                return new ApiResponse("Email already confirmed.");
            }

            var token = Encoding.UTF8.GetString(Convert.FromBase64String(confirmEmailDto.Token));
            await _userManager.ConfirmEmailAsync(user, token);
            return new ApiResponse($"Account Confirmed for {user.Email}.");
        }

        public async Task<ApiResponse> CreateAsync(UserCreateDTO userCreateDto)
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

            var itemsModuleId = ModuleSeed.Data.First(m => m.Name == "Items").Id;
            var readPermissionId = PermissionSeed.Data.First(p => p.Name == Permissions.Read).Id;

            var modulePermission = new List<ModulePermissionAddDTO>
                {
                    new ModulePermissionAddDTO
                    {
                        ModuleId = itemsModuleId,
                        PermissionIds = new List<int> { readPermissionId }
                    }
                };

            await _userServiceAsync.AddModulesAsync(Guid.Parse(user.Id), modulePermission);

            var emailVerificationLink = await GenerateEmailVerificationLinkAsync(user);

            var emailDto = CreateConfirmationEmail(user, emailVerificationLink);
            await _emailServiceAsync.SendEmailAsync(emailDto);

            var message = $"User registered successfully. A confirmation email has been sent to {user.Email}. Please confirm your account by visiting the URL provided in the email.";
            return new ApiResponse(message);
        }

        public async Task ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            // always return ok response to prevent email enumeration
            if (user == null || !user.EmailConfirmed) return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            var baseUrl = $"{_jwtSettings.Audience}/reset-password";
            var resetPasswordUrl = QueryHelpers.AddQueryString(baseUrl, "token", encodedToken);
            resetPasswordUrl = QueryHelpers.AddQueryString(resetPasswordUrl, "userId", user.Id);

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
                            <p>If you did not request this, please ignore this email.</p>
                            <p>Best regards,<br>Your Company Team</p>
                        </body>
                        </html>",
                To = model.Email,
                Subject = "Reset Password",
            };
            await _emailServiceAsync.SendEmailAsync(emailRequest);
        }

        public async Task<ApiResponse<AuthenticationResponseDTO>> LoginAsync(LoginDto loginDto, string ipAddress)
        {

            var user = await FindByEmailAsync(loginDto.Email);

            if (!user.EmailConfirmed)
            {
                throw new BadRequestException($"Email for user '{loginDto.Email}' is not confirmed.");
            }

            if (user.LockoutEnd is not null)
            {
                throw new BadRequestException($"User account '{loginDto.Email}' is locked.");
            }

            if (!user.Active)
            {
                throw new BadRequestException($"User account '{loginDto.Email}' is inactivated.");
            }


            var result = await _singInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: true, lockoutOnFailure: true); ;

            if (result.Succeeded)
            {
                JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
                AuthenticationResponseDTO response = new AuthenticationResponseDTO();
                response.Id = user.Id;
                response.AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                response.Email = user.Email;
                response.UserName = user.UserName;
                response.Roles = _userRoles;
                response.Modules = await _userModulePermissionRepositoryAsync.GetModulePermissionsAsync(user.Id);
                response.IsVerified = user.EmailConfirmed;
                var refreshToken = GenerateRefreshToken(ipAddress);
                response.RefreshToken = refreshToken.Token;
                return new ApiResponse<AuthenticationResponseDTO>(response);
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

        public async Task<ApiResponse> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto)
        {

            var user = await _userManager.FindByEmailAsync(resendConfirmationEmailDto.Email);

            if (user.EmailConfirmed)
            {
                throw new BadRequestException("Email is already confirmed.");
            }

            var emailVerificationLink =  await GenerateEmailVerificationLinkAsync(user);
            var emailDto = CreateConfirmationEmail(user, emailVerificationLink);

            await _emailServiceAsync.SendEmailAsync(emailDto);
            return new ApiResponse("A new confirmation email has been sent. Please check your email.");
        }


        public async Task<ApiResponse> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null) throw new NotFoundException("User", model.UserId);

            var token = Encoding.UTF8.GetString(Convert.FromBase64String(model.Token));
            await _userManager.ResetPasswordAsync(user, token, model.Password);
            return new ApiResponse($"Password Reset Successful for {user.UserName}.");
        }

        private EmailDto CreateConfirmationEmail(User user, string emailConfirmationLink)
        {
            var tokenDuration =_jwtSettings.EmailConfirmationTokenDurationInHours;
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

        private async Task<User> FindByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                throw new NotFoundException("User", userId);
            }

            return user;
        }

        private async Task<User> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new NotFoundException($"User with email '{email}' not found.");
            }

            return user;
        }

        private async Task<string> GenerateEmailVerificationLinkAsync(User user)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailConfirmationToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailConfirmationToken));
            var httpRequest = _httpContext?.HttpContext?.Request;
            var currentRequestUrl = httpRequest != null ? httpRequest.GetEncodedUrl() : string.Empty;

            var apiBaseUrl = currentRequestUrl.Substring(0, currentRequestUrl.LastIndexOf("/auth", StringComparison.OrdinalIgnoreCase)+ 5);
            var emailConfirmationUrl = $"{apiBaseUrl}/confirm-email";

            var emailVerificationLink = QueryHelpers.AddQueryString(emailConfirmationUrl, "userId", user.Id);
            emailVerificationLink = QueryHelpers.AddQueryString(emailVerificationLink, "token", encodedEmailConfirmationToken);

            return emailVerificationLink;
        }

        private async Task<JwtSecurityToken> GenerateJWToken(User user)
        {
            var userClaimList = await _userManager.GetClaimsAsync(user);
            _userRoles = (List<string>)await _userManager.GetRolesAsync(user);
            var roles = new List<Claim>();

            foreach (var role in _userRoles)
            {
                roles.Add(new Claim("roles", role));
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userId", user.Id),
                new Claim("ip", ipAddress)
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

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}