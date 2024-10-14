using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Application.Utilities;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.Entities;
using Template.Identity.Specification.V1;

namespace Template.Identity.Services.V1
{
    public class UserServiceAsync : IUserServiceAsync
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly IUserRepositoryAsync _userRepository;
        private readonly UserManager<User> _userManager;

        public UserServiceAsync(
            IUserRepositoryAsync userRepository,
            IMapper mapper,
            IEmailServiceAsync emailService,
            IHttpContextAccessor httpContext,
            IResourceServiceAsync moduleService,
            UserManager<User> userManager
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
        }



        public async Task AddToRolesAsync(Guid userId, List<Guid> roles)
        {
            var user = await GetUser(userId);

            var userRoles = roles.Select(role => new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.ToString()
            }).ToList();

            await _userRepository.AddToRolesAsync(userRoles);
        }

        public async Task AddToRolesAsync(Guid userId, Guid roleId)
        {
            var user = await GetUser(userId);
            var userRole = new IdentityUserRole<string> { UserId = user.Id, RoleId = roleId.ToString() };
            await _userRepository.AddToRoleAsync(userRole);
        }

       

        public async Task DeleteAsync(Guid userId)
        {
            var user = await GetUser(userId);
            user.IsDeleted = true;
            await _userManager.UpdateAsync(user);
        }

        public async Task<ApiResponse<PagedCollection<UserListDTO>>> GetAllAsync(UserFilterRequest request)
        {
            var spec = new UserSpecification(request);

            if (spec.Skip % spec.Take != 0)
            {
                throw new BadRequestException($"The 'offset' value ({spec.Skip}) must be either zero or a multiple of the 'limit' value({spec.Take}).");
            }

            var total = await _userRepository.GetTotalRecordsAsync(spec.Criteria);

            if (total < spec.Skip)
            {
                throw new BadRequestException($"The 'offset' value must be either zero or minimum to 'total' value ({total}) and multiple of the 'limit' value({spec.Take}).");
            }

            var userList = await _userRepository.GetAllProjectedWithPaginationAsync(spec);

            var href = _httpContext?.HttpContext?.Request.GetEncodedUrl();
            var next = UrlUtile.GetNextURL(href, request.Limit, request.Offset, total);
            var prev = UrlUtile.GetPrevURL(href, request.Limit, request.Offset);

            var data = new PagedCollection<UserListDTO>
            {
                Elements = userList,
                HRef = href,
                Prev = prev,
                Next = next,
                Offset = request.Offset,
                Limit = request.Limit,
                Total = total
            };

            return new ApiResponse<PagedCollection<UserListDTO>>(data);
        }

        public async Task<ApiResponse<UserDetailDTO>> GetByIdAsync(Guid userId)
        {
            var user = await GetUser(userId);
            var roles = (List<string>) await _userManager.GetRolesAsync(user);
            var userDTO = _mapper.Map<UserDetailDTO>(user);
            userDTO.Roles = roles;
            return new ApiResponse<UserDetailDTO>(userDTO);
        }


        public async Task<ApiResponse<List<string>>> GetRolesAsync(Guid userId)
        {
            var user = await GetUser(userId);
            var roles = (List<string>) await _userManager.GetRolesAsync(user);
            return new ApiResponse<List<string>>(roles);
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            return await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
        }


        public async Task RemoveFromRolesAsync(Guid userId, List<Guid> roles)
        {
            var user = await GetUser(userId);
            var userRoles = roles.Select(role => new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.ToString()
            }).ToList();

            await _userRepository.RemoveFromRolesAsync(userRoles);
        }

        public async Task RemoveFromRolesAsync(Guid userId, Guid roleId)
        {
            var user = await GetUser(userId);
            var userRole = new IdentityUserRole<string> { UserId = user.Id, RoleId = roleId.ToString() };
            await _userRepository.RemoveFromRoleAsync(userRole);
        }


        public async Task UpdateAsync(Guid userId, UserUpdateDTO userUpdateDTO)
        {
            var currentUserId = _httpContext?.HttpContext?.GetUserId();

            if (userId != userUpdateDTO.Id)
            {
                throw new BadRequestException("The provided user ID does not match the ID in the request data.");
            }

            if (currentUserId != userUpdateDTO.Id)
            {
                throw new ForbiddenException();
            }

            var user = await GetUser(userId);
            _mapper.Map(userUpdateDTO, user);
            await _userManager.UpdateAsync(user);
        }

        #region Private Methods

        private async Task<User> GetUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            return user;
        }





        #endregion Private Methods
    }

}