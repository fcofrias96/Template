using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.DTOs.Auth;
using Template.Core.Application.DTOs.Email;
using Template.Core.Application.Extensions;
using Template.Core.Domain.Exceptions;
using Template.Infrastructure.Identity.Contracts.Repositories;
using Template.Infrastructure.Identity.Contracts.Services;
using Template.Infrastructure.Identity.Entity;
using Template.Infrastructure.Identity.Helpers;
using Template.Infrastructure.Identity.Settings;
using ILogger = Serilog.ILogger;

namespace Template.Infrastructure.Identity.Services
{
    public class AuthServiceAsync : IAuthServiceAsync
    {
        private readonly IUserRepositoryAsync _userRepositoryAsync;
        private readonly IRoleRepositoryAsync _roleRepositoryAsync;
        private readonly IAuthRepositoryAsync _authRepositoryAsync;
        private readonly IEmailServiceAsync _emailServiceAsync;
        private readonly IHttpContextAccessor _httpContext;
        private readonly JWTSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthServiceAsync(IUserRepositoryAsync userRepositoryAsync,
            IAuthRepositoryAsync authRepositoryAsync,
            IEmailServiceAsync emailServiceAsync,
            IRoleRepositoryAsync roleRepositoryAsync,
            IHttpContextAccessor httpContext,
            IOptions<JWTSettings> jwtSettings,
            ILogger logger,
            IConfiguration configuration
            )
        {
            _userRepositoryAsync = userRepositoryAsync;
            _authRepositoryAsync = authRepositoryAsync;
            _roleRepositoryAsync = roleRepositoryAsync;
            _emailServiceAsync = emailServiceAsync;
            _httpContext = httpContext;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> ConfirmEmailAsync(ConfirmEmailDTO confirmEmailDTO)
        {
            var user = await _userRepositoryAsync.FindByIdAsync(confirmEmailDTO.UserId);
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailDTO.Code));
            var result = await _authRepositoryAsync.ConfirmEmailAsync(user, code);
            var baseUrl = _httpContext?.HttpContext?.Request.GetBaseUrl();
            if (result.Succeeded)
            {
                return $"Account Confirmed for {user.Email}. You can now use the {baseUrl}auth/login endpoint.";
            }
            else
            {
                throw new Exception($"An error occured while confirming {user.Email}.");
            }
        }

        public async Task ForgotPassword(ForgotPasswordDTO model)
        {
            var account = await _userRepositoryAsync.FindByEmailAsync(model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            var code = await _authRepositoryAsync.GeneratePasswordResetTokenAsync(account);
            var url = _httpContext?.HttpContext?.Request.GetEncodedUrl();
            var resetPasswordUrl = url?.Replace("forgot", "reset");
            var emailRequest = new EmailDTO()
            {
                Body = $"You reset token is  {code}\n Enter to this endpoint {resetPasswordUrl} to reset your password",
                To = model.Email,
                Subject = "Reset Password",
            };
            await _emailServiceAsync.SendAsync(emailRequest);
        }

        public async Task<AuthenticationResponseDTO> LoginAsync(LoginDTO request, string ipAddress)
        {
            User user;
            user = await _userRepositoryAsync.FindByNameAsync(request.UserName);
            if (user == null)
            {
                user = await _userRepositoryAsync.FindByEmailAsync(request.UserName);
                if (user == null)
                {
                    throw new NotFoundException($"No Accounts Registered with {request.UserName}.");
                }
            }
            var result = await _authRepositoryAsync.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new BadRequestException($"Invalid Credentials for '{request.UserName}'.");
            }
            if (!user.EmailConfirmed)
            {
                throw new BadRequestException($"Account Not Confirmed for '{request.UserName}'.");
            }
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponseDTO response = new AuthenticationResponseDTO();
            response.Id = user.Id;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            var rolesList = await _userRepositoryAsync.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return response;
        }

        public async Task<string> ResetPassword(ResetPasswordDTO model)
        {
            var account = await _userRepositoryAsync.FindByEmailAsync(model.Email);
            if (account == null) throw new NotFoundException($"No Accounts Registered with {model.Email}.");
            var result = await _authRepositoryAsync.ResetPasswordAsync(account, model.Token, model.Password);
            if (result.Succeeded)
            {
                return "Password Resetted.";
            }
            else
            {
                throw new Exception($"Error occured while reseting the password.");
            }
        }

        private async Task<JwtSecurityToken> GenerateJWToken(User user)
        {
            var userClaimList = await _userRepositoryAsync.GetClaimsAsync(user);
            var roles = await _userRepositoryAsync.GetRolesAsync(user);
            var roleClaimList = new List<Claim>();
            var permissionClaimList = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaimList.Add(new Claim("roles", role));
                var roleclaims = await _roleRepositoryAsync.GetClaims(role);
                foreach (var claim in roleclaims)
                {
                    if (!permissionClaimList.Exists(x => x.Value == claim.Value && x.Type == claim.Type))
                    {
                        permissionClaimList.Add(claim);
                    }
                }
            }

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress),
            };

            claims.AddRange(userClaimList);
            claims.AddRange(roleClaimList);
            claims.AddRange(permissionClaimList);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
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
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}