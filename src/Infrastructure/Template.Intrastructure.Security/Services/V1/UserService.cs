using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Template.Application.Contracts.Caching.Invalidators;
using Template.Application.Exceptions;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Application.Services.Base;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services;
using Template.Security.Contracts.Services.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class UserService : BaseService<ApplicationUser>, IUserService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserAuthCacheInvalidator _userAuthCacheInvalidator;
        private readonly IPermissionCacheInvalidator _permissionCacheInvalidator;
        private readonly IMenuCacheInvalidator _menuCacheInvalidator;


        public UserService(
            IResourceService resourceService,
            UserManager<ApplicationUser> userManager,
            IUserRoleRepository userRoleRepository,
            IUserAuthCacheInvalidator userAuthCacheInvalidator,
            IPermissionCacheInvalidator permissionCacheInvalidator,
            IMenuCacheInvalidator menuCacheInvalidator,
            ICurrentUserService currentUserService,
            BaseServiceDeps<ApplicationUser> deps

        )
            : base(deps)
        {
            _userRepository = (IUserRepository) deps.Repository;
            _mapper = deps.Mapper;
            _userManager = userManager;
            _userRoleRepository = userRoleRepository;
            _userAuthCacheInvalidator = userAuthCacheInvalidator;
            _permissionCacheInvalidator = permissionCacheInvalidator;
            _menuCacheInvalidator = menuCacheInvalidator;
            _currentUserService = currentUserService;
        }

        public async Task DeleteAsync(int userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await GetUser(userId);
            await _userRepository.SoftDeleteAsync(user, cancellationToken);

        }

        public async Task<ApiResponse<IReadOnlyList<RoleListDto>>> GetRolesAsync(int userId, CancellationToken cancellationToken)
        {
            var roles = await _userRepository.GetRolesAsync(userId, cancellationToken);
            return new ApiResponse<IReadOnlyList<RoleListDto>>(roles);
        }

        public async Task<ApplicationUser?> GetByRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            return await _userRepository.GetByRefreshTokenAsync(refreshToken, cancellationToken);
        }

        public async Task UpdateAsync(int userId, UserUpdateDto userUpdateDTO, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

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

        public async Task<UserDetailDto> GetDetailAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetDetailAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }
            return user;
        }

        public async Task<ResourcePermissionDto[]> GetResourcesAsync(int userId, CancellationToken cancellationToken)
        {
            return await _userRepository.GetResourcesAsync(userId, cancellationToken);
        }

        public async Task UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, CancellationToken cancellationToken)
        {

            await _userRoleRepository.UpdateUserRolesAsync(userId, roleIds, cancellationToken);
            _permissionCacheInvalidator.InvalidateUser(userId);
            _userAuthCacheInvalidator.InvalidateUser(userId);
            _menuCacheInvalidator?.InvalidatePermissions();
        }

        #region Private Methods

        private async Task<ApplicationUser> GetUser(int userId)
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