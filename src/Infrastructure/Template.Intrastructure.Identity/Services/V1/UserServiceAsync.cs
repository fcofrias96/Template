using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Template.Application.Contracts;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Identity.Contracts.Repositories.V1;
using Template.Identity.Contracts.Services.V1;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Services.V1
{
    public class UserServiceAsync : IUserServiceAsync
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly IReadServiceAsync<User, IdentityContext> _readService;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepositoryAsync _userRepository;
        public UserServiceAsync(
            IUserRepositoryAsync userRepository,
            IMapper mapper,
            IEmailServiceAsync emailService,
            IHttpContextAccessor httpContext,
            IResourceServiceAsync moduleService,
            UserManager<User> userManager,
            IReadServiceAsync<User, IdentityContext> readService
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
            _readService = readService;
        }

        public async Task AddToRolesAsync(Guid userId, IReadOnlyList<Guid> roles)
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

        public async Task<User> GetByIdAsync<TKey>(TKey id)
        {
            var parsedId = ParseToString(id);
            return await _readService.GetByIdAsync(parsedId);
        }
        public async Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id)
        {
            var parsedId = ParseToString(id);
            return await _readService.GetByIdProjectedAsync<string, TDestination>(parsedId);
        }

        public async Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>()
        {
            return await _readService.GetProjectedAsync<TDestination>();
        }

        public async Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters) where TSpecification : ISpecification<User>
        {
            return await _readService.GetProjectedPaginatedAsync<TDestination, TSpecification>(queryParameters);
        }

        public async Task<ApiResponse<IReadOnlyList<RoleListDto>>> GetRolesAsync(Guid userId)
        {
            var parsedId = ParseToString(userId);

            var roles = await _userRepository.GetRoles(parsedId);
            return new ApiResponse<IReadOnlyList<RoleListDto>>(roles);
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            return await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
        }

        public async Task RemoveFromRolesAsync(Guid userId, IReadOnlyList<Guid> roles)
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

        public async Task UpdateAsync(Guid userId, UserUpdateDto userUpdateDTO)
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


        public async Task<UserDetailDto> GetUserDetailAsync(Guid userId)
        {
            var user = await _userRepository.GetUserDetailAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }
            return user;
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


        private static string ParseToString<TKey>(TKey id)
        {
            return id?.ToString() ?? string.Empty;
        }
        #endregion Private Methods
    }
}