using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.DTOs.V1.Module;
using Template.Application.Models.DTOs.V1.ModulePermission;
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
        private readonly IModuleService _moduleService;
        private readonly IUserModulePermissionRepositoryAsync _userModulePermissionRepositoryAsync;
        private readonly IUserRepositoryAsync _userRepository;
        private readonly UserManager<User> _userManager;

        public UserServiceAsync(
            IUserRepositoryAsync userRepository,
            IMapper mapper,
            IEmailServiceAsync emailService,
            IHttpContextAccessor httpContext,
            IModuleService moduleService,
            IUserModulePermissionRepositoryAsync userModulePermissionRepositoryAsync,
            UserManager<User> userManager
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContext = httpContext;
            _moduleService = moduleService;
            _userModulePermissionRepositoryAsync = userModulePermissionRepositoryAsync;
            _userManager = userManager;
        }

        public async Task AddModulesAsync(Guid userId, List<ModulePermissionAddDTO> modulePermissions)
        {
            var user = await GetUser(userId);
            var userModulePermissions = new List<UserModulePermission>();

            foreach (var modulePermission in modulePermissions)
            {
                foreach (var permission in modulePermission.PermissionIds)
                {
                    var userModulePermission = new UserModulePermission
                    {
                        UserId = user.Id,
                        ModuleId = modulePermission.ModuleId,
                        PermissionId = permission
                    };

                    userModulePermissions.Add(userModulePermission);
                }
            }

            await _userModulePermissionRepositoryAsync.CreateRangeAsync(userModulePermissions);
        }

        public async Task AddPermissionsFromModuleAsync(Guid userId, int moduleId, List<int> permissionIds)
        {
            var user = await GetUser(userId);
            var userModulePermissions = permissionIds.Select(permissionId => new UserModulePermission
            {
                UserId = user.Id,
                ModuleId = moduleId,
                PermissionId = permissionId
            }).ToList();

            await _userModulePermissionRepositoryAsync.CreateRangeAsync(userModulePermissions);
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
            user.Active = false;
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

        public async Task<ApiResponse<ModulePermissionDTO>> GetModuleAsync(Guid userId, int moduleId)
        {
            var user = await GetUser(userId);
            return await _moduleService.GetModulePermissionsAsync(user.Id, moduleId);
        }

        public async Task<ApiResponse<List<ModulePermissionDTO>>> GetModulesAsync(Guid userId)
        {
            var user = await GetUser(userId);
            var modulePermissions = await _userModulePermissionRepositoryAsync.GetModulePermissionsAsync(user.Id);
            return new ApiResponse<List<ModulePermissionDTO>>(modulePermissions);
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

        public async Task<bool> IsAuthorized(string userId, string module, string permission)
        {
            if(string.IsNullOrEmpty(userId)) return false;

            return await _userModulePermissionRepositoryAsync.IsAuthorizedAsync(userId, module, permission);
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

        public async Task RemoveModuleAsync(Guid userId, int moduleId)
        {
            var user = await GetUser(userId);
            var userModulePermissions = await _userModulePermissionRepositoryAsync.GetByUserIdAndModuleIdAsync(user.Id, moduleId);
            await _userModulePermissionRepositoryAsync.DeleteRangeAsync(userModulePermissions);
        }

        public async Task RemovePermissionsFromModuleAsync(Guid userId, int moduleId, List<int> permissionIds)
        {
            var user = await GetUser(userId);
            var userModulePermission = await _userModulePermissionRepositoryAsync.GetAllAsync(user.Id, moduleId, permissionIds);
            await _userModulePermissionRepositoryAsync.DeleteRangeAsync(userModulePermission);
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