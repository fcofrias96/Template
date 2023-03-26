using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using Template.Core.Application.Contracts.Services;
using Template.Core.Application.DTOs.Email;
using Template.Core.Application.DTOs.User;
using Template.Core.Application.Extensions;
using Template.Core.Application.Utils;
using Template.Core.Application.Wrappers;
using Template.Core.Domain.Constants;
using Template.Core.Domain.Entities;
using Template.Core.Domain.Enums;
using Template.Core.Domain.Exceptions;
using Template.Infrastructure.Identity.Contracts.Repositories;
using Template.Infrastructure.Identity.Contracts.Services;
using Template.Infrastructure.Identity.Entity;

namespace Template.Infrastructure.Identity.Services
{
    public class UserServiceAsync : IUserServiceAsync
    {
        private readonly IAuthRepositoryAsync _authRepositoryAsync;
        private readonly IEmailServiceAsync _emailService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly IUserRepositoryAsync _userRepository;
        public UserServiceAsync(IUserRepositoryAsync userRepository, 
            IMapper mapper, 
            IEmailServiceAsync emailService,
            IAuthRepositoryAsync authRepositoryAsync,
            IHttpContextAccessor httpContext
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _authRepositoryAsync = authRepositoryAsync;
            _httpContext = httpContext;
        }


        public async Task<Response<string>> AddAsync(UserCreateDTO userCreateDTO)
        {
            var userWithSameUserName = await _userRepository.FindByNameAsync(userCreateDTO.UserName);
            if (userWithSameUserName != null)
            {
                throw new BadRequestException($"Username '{userCreateDTO.UserName}' is already taken.");
            }
            var user = new User
            {
                Email = userCreateDTO.Email,
                FirstName = userCreateDTO.FirstName,
                LastName = userCreateDTO.LastName,
                UserName = userCreateDTO.UserName
            };
            var userWithSameEmail = await _userRepository.FindByEmailAsync(userCreateDTO.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userRepository.CreateAsync(user, userCreateDTO.Password);
                if (result.Succeeded)
                {
                    await _userRepository.AddToRoleAsync(user, Role.Basic);
                    var permissionClaim = new Claim("Permissions", "Read");
                    await _userRepository.AddClaimAsync(user, permissionClaim);


                    var verificationUri = await SendVerificationEmail(user);
                    //TODO: Attach Email Service here and configure it via appsettings
                    await _emailService.SendAsync(new EmailDTO()
                    {
                        From = "mail@codewithmukesh.com",
                        To = user.Email,
                        Body = $"Please confirm your account by visiting this URL {verificationUri}",
                        Subject = "Confirm Registration"
                    });
                    return new Response<string>(user.Id, message: $"User Registered. Please confirm your account by visiting this URL {verificationUri}");
                }
                else
                {
                    throw new Exception($"{result.Errors}");
                }
            }
            else
            {
                throw new BadRequestException($"Email {userCreateDTO.Email} is already registered.");
            }
        }

        public async Task AddToRolesAsync(Guid id, List<string> roles)
        {
            var user = await _userRepository.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            var result = await _userRepository.AddToRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", id); 
            }

                await _userRepository.DeleteAsync(user);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var user = await _userRepository.FindByIdAsync(id.ToString());
            return user != null;
        }


        public async Task<PagedResult<UserListDTO>> GetAllAsync(QueryParameters<User> queryParameters)
        {
            var userList = await _userRepository.GetAllAsync(queryParameters);
            var total = await _userRepository.GetTotalRecordsAsync();
            var href = _httpContext?.HttpContext?.Request.GetEncodedUrl();
            var next = UrlUtils.GetNextURL(href, queryParameters.Limit, queryParameters.Offset, total);
            var prev = UrlUtils.GetPrevURL(href, queryParameters.Limit, queryParameters.Offset, total);

            return new PagedResult<UserListDTO>
            {
                Items = userList,
                Href = href,
                Prev = prev,
                Next = next,
                Offset = queryParameters.Offset,
                Limit = queryParameters.Limit,
                Total = total
            };

        }

        public async Task<UserDTO> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }
            var roles = (List<string>)await _userRepository.GetRolesAsync(user);
            var userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Roles = roles;
            return userDTO;
        }

        public async Task RemoveFromRolesAsync(Guid id, List<string> roles)
        {

            var user = await _userRepository.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            var result = await _userRepository.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }

        public async Task UpdateAsync(Guid id, UserUpdateDTO userUpdateDTO)
        {
            var userId = _httpContext?.HttpContext?.GetUserId();
            
            if(id != userUpdateDTO.Id)
            {
                throw new BadRequestException("Invalid Id used in request");
            }

            if(userId != id)
            {
                throw new BadRequestException("Unauthorized to access to this resorce");
            }

            var user = await _userRepository.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }


            if(userUpdateDTO.CurrentPassword != null && userUpdateDTO.NewPassword != null && userUpdateDTO.ConfirmPassword != null) 
            {
                var result = await _userRepository.ChangePasswordAsync(user, userUpdateDTO.CurrentPassword, userUpdateDTO.NewPassword);
                
                if (!result.Succeeded) 
                {
                    throw new Exception(result.Errors.First().Description);
                
                }
            }

            _mapper.Map(userUpdateDTO, user);
            await _userRepository.UpdateAsync(user);
        }

        public async Task AddPermissionsAsync( Guid userId, List<string> permissions)
        {
            var user = await _userRepository.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            var joinedPermissions =  string.Join<string>(',',permissions);
            var permissionClaim = new Claim("Permissions", joinedPermissions);

            await _userRepository.AddClaimAsync(user, permissionClaim);
        }

        #region Private Methods
        private async Task<string> SendVerificationEmail(User user)
        {
            var code = await _authRepositoryAsync.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var baseUrl = _httpContext?.HttpContext?.Request.GetBaseUrl();
            var url = baseUrl + "auth/confirm-email/";
            var enpointUri = new Uri(url);
            var verificationUri = QueryHelpers.AddQueryString(enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }

        public async Task<IList<string>> GetRolesAsync(Guid id)
        {
            var user = await _userRepository.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            return await _userRepository.GetRolesAsync(user);
        }

        public async Task<IList<Claim>> GetPermissionsAsync(Guid id)
        {
            var user = await _userRepository.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            return await _userRepository.GetClaimsAsync(user);
        }



        #endregion Private Methods





        /* public async Task<Response<UserDetailDTO>> LoginAsync(LoginDTO userCreateDTO)
         {
             _user = await _userRepository.FindByNameAsync(userCreateDTO.UserName);

             if (_user == null)
             {
                 throw new NotFoundException(userCreateDTO.UserName);
             }

             var result = await _signInManager.PasswordSignInAsync(_user.UserName, userCreateDTO.Password, false, lockoutOnFailure: false);

             if (!result.Succeeded)
             {
                 throw new BadRequestException($"Invalid Password for {userCreateDTO.UserName}");
             }

             if (!_user.EmailConfirmed)
             {
                 throw new BadRequestException($"Account Not Comfirmed for {_user.Email}");
             }

             var accessToken = await GenerateAccessTokenAsync();
             var refreshToken = await CreateRefreshTokenAsync();
             var roleList = (List<string>)await _userRepository.GetRolesAsync(_user);

             var authentication = new UserDetailDTO
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

             var response = new Response<UserDetailDTO>(authentication);
             response.Message = "Successs Authentication.";

             return response;
         }

         private async Task<string> CreateRefreshTokenAsync()
         {
             await _userRepository.RemoveAuthenticationTokenAsync(_user, Token.Provider, Token.Name);
             var refreshToken = await _userRepository.GenerateUserTokenAsync(_user, Token.Provider, Token.Name);
             var result = await _userRepository.SetAuthenticationTokenAsync(_user, Token.Provider, Token.Name, refreshToken);
             return refreshToken;
         }



         public async Task<IEnumerable<IdentityError>> Register(UserCreateDTO userCreateDTO)
         {

         }

          private async Task<string> GenerateAccessTokenAsync()
         {
             var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
             var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
             var roleList = await _userRepository.GetRolesAsync(_user);

             var userClaims = await _userRepository.GetClaimsAsync(_user);
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

         public async Task<Response<RefreshTokenResponseDTO?>> VerifyRefreshToken(RefreshTokenRequestDTO userCreateDTO)
         {
             var jwtTokenHandler = new JwtSecurityTokenHandler();
             var tokenContent = jwtTokenHandler.ReadJwtToken(userCreateDTO.AccessToken);
             var userName = tokenContent.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
             _user = await _userRepository.FindByEmailAsync(userName);

             if (_user == null)
             {
                 return default;
             }

             var isValidRefreshToken = await _userRepository.VerifyUserTokenAsync(_user, Token.Provider, Token.Name, userCreateDTO.RefreshToken);
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

             await _userRepository.UpdateSecurityStampAsync(_user);
             return default;
         }*/


    }
}