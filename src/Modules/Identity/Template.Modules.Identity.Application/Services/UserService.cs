using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Template.BuildingBlocks.Application.Caching;
using Template.Modules.Identity.Application.Caching;
using Template.Modules.Identity.Application.Dtos.Resources;
using Template.Modules.Identity.Application.Dtos.Roles;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.BuildingBlocks.Application.Services.Base;
using Template.Modules.Identity.Application.Contracts.Repositories;
using Template.Modules.Identity.Application.Contracts.Services;
using Template.Modules.Identity.Application.Errors;
using Template.Modules.Identity.Domain.Entities;
using Template.SharedKernel.Results;

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

        public async Task<Result> DeleteAsync(int userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userResult = await GetUserAsync(userId);
            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            await _userRepository.SoftDeleteAsync(userResult.Value, cancellationToken);
            return Result.Success();
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

        public async Task<Result> UpdateAsync(int userId, UserUpdateDto userUpdateDTO, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            if (userId != userUpdateDTO.Id)
            {
                return UserErrors.IdMismatch;
            }

            if (currentUserId != userUpdateDTO.Id)
            {
                return UserErrors.Forbidden;
            }

            var userResult = await GetUserAsync(userId);
            if (userResult.IsFailure)
            {
                return userResult.Error;
            }

            var user = userResult.Value;
            userUpdateDTO.ApplyTo(user);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return UserErrors.UpdateFailed(updateResult);
            }

            return Result.Success();
        }

        public async Task<Result<UserDetailDto>> GetDetailAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetDetailAsync(userId, cancellationToken);
            if (user is null)
            {
                return UserErrors.NotFound(userId);
            }

            return Result.Success(user);
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

        private async Task<Result<ApplicationUser>> GetUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return UserErrors.NotFound(userId);
            }

            return Result.Success(user);
        }

        #endregion Private Methods
    }
}
