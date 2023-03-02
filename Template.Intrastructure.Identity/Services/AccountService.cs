using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Template.Infrastructure.Identity.Models;
using Template.Core.Application.Constants;
using Template.Core.Application.DTOs.Auth;
using Template.Core.Application.DTOs.Token;
using Template.Core.Application.DTOs.User;
using Template.Core.Application.Exceptions;
using Template.Core.Application.Interfaces.Services;
using Template.Core.Application.Wrappers;
using Template.Core.Domain.Settings;

namespace Template.Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private ApplicationUser _user;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JWTSettings> jwtSettings,
            ILogger logger,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Response<AuthenticationResponse>> LoginAsync(AuthenticationRequest request)
        {
            _user = await _userManager.FindByNameAsync(request.UserName);

            if (_user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.UserName}.");
            }

            var result = await _signInManager.PasswordSignInAsync(_user.UserName, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid Password for {request.UserName}");
            }

            if (!_user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Comfirmed for {_user.Email}");
            }

            var accessToken = await GenerateAccessTokenAsync();
            var refreshToken = await CreateRefreshTokenAsync();
            var roleList = (List<string>)await _userManager.GetRolesAsync(_user);

            var authentication = new AuthenticationResponse
            {
                Id = _user.Id,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Email = _user.Email,
                UserName = _user.UserName,
                IsVerified = _user.EmailConfirmed,
                Roles = roleList,
                ExpiresIn = _jwtSettings.DurationInMinutes
            };

            _logger.Information("User {UserName} logged in", _user.UserName);

            var response = new Response<AuthenticationResponse>(authentication);
            response.Message = "Successs Authentication.";

            return response;
        }

        private async Task<string> CreateRefreshTokenAsync()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, Token.Provider, Token.Name);
            var refreshToken = await _userManager.GenerateUserTokenAsync(_user, Token.Provider, Token.Name);
            var result = await _userManager.SetAuthenticationTokenAsync(_user, Token.Provider, Token.Name, refreshToken);
            return refreshToken;
        }

        public Task<RefreshTokenResponseDTO> Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDTO apilUserDTO)
        {
            _user = _mapper.Map<ApplicationUser>(apilUserDTO);
            _user.UserName = apilUserDTO.UserName;

            var reuslt = await _userManager.CreateAsync(_user, apilUserDTO.Password);
            if (reuslt.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, Role.Basic);
            }

            return reuslt.Errors;
        }

        private async Task<string> GenerateAccessTokenAsync()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roleList = await _userManager.GetRolesAsync(_user);

            var userClaims = await _userManager.GetClaimsAsync(_user);
            var rolesClaims = roleList.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid", _user.Id)
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var securityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        public async Task<Response<RefreshTokenResponseDTO?>> VerifyRefreshToken(RefreshTokenRequestDTO request)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtTokenHandler.ReadJwtToken(request.AccessToken);
            var userName = tokenContent.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByEmailAsync(userName);

            if (_user == null)
            {
                return default;
            }

            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, Token.Provider, Token.Name, request.RefreshToken);
            if (isValidRefreshToken)
            {
                var accessToken = await GenerateAccessTokenAsync();
                var tokenResponse = new RefreshTokenResponseDTO
                {
                    AccessToken = accessToken,
                    ExpiresIn = _jwtSettings.DurationInMinutes,
                    RefreshToken = await CreateRefreshTokenAsync()
                };

                var response = new Response<RefreshTokenResponseDTO>(tokenResponse);
                response.Message = "Successs Refresh AccessToken.";
                _logger.Information("Access Token refreshed to user {UseName}", _user.UserName);
                return response;
            }

            await _userManager.UpdateSecurityStampAsync(_user);
            return default;
        }
    }
}