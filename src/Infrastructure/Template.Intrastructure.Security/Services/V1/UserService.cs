using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Template.Application.Contracts;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Application.Services.Base;
using Template.Domain.Entities.V1;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class UserService : BaseService<ApplicationUser>,  IUserService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly ICacheService _cache;
        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IEmailService emailService,
            IHttpContextAccessor httpContext,
            IResourceService moduleService,
            UserManager<ApplicationUser> userManager,
            ICacheService memoryCache,
            IUserRoleRepository userRoleRepository,
            ICacheService cache
            ) : base(memoryCache, mapper, userRepository, httpContext)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
            _userRoleRepository = userRoleRepository;
            _cache = cache;
        }

        public async Task DeleteAsync(int userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await GetUser(userId);
            user.MarkDeleted();
            await _userManager.UpdateAsync(user);
        }

        public async Task<ApiResponse<IReadOnlyList<RoleListDto>>> GetRolesAsync(int userId, CancellationToken cancellationToken)
        {
            var roles = await _userRepository.GetRolesAsync(userId, cancellationToken);
            return new ApiResponse<IReadOnlyList<RoleListDto>>(roles);
        }

        public async Task<ApplicationUser?> GetUserByRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            return await _userRepository.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
        }

        public async Task UpdateAsync(int userId, UserUpdateDto userUpdateDTO, CancellationToken cancellationToken)
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

        public async Task<UserDetailDto> GetUserDetailAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserDetailAsync(userId, cancellationToken);
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

            _cache.Remove(PermissionService.BuildPermissionsCacheKey(userId));
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