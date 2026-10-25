using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Identity.Application.Caching;
using Template.BuildingBlocks.Application.Exceptions;
using Template.Modules.Identity.Application.Dtos.Resources;
using Template.Modules.Identity.Application.Dtos.Roles;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Application.Services.Base;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Services
{
    public class UserService : BaseService<ApplicationUser>, IUserService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICacheService _cache;

        public UserService(
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            ICurrentUserService currentUserService,
            ICacheService cache,
            IHttpContextAccessor httpContext)
            : base(userRepository, cache, httpContext)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _userRoleRepository = userRoleRepository;
            _currentUserService = currentUserService;
            _cache = cache;
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
            userUpdateDTO.ApplyTo(user);
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

            // The user's permissions and roles changed: drop what was cached from the old ones.
            _cache.Remove(IdentityCacheKeys.UserPermissions(userId));
            _cache.Remove(IdentityCacheKeys.UserAuthData(userId));
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
