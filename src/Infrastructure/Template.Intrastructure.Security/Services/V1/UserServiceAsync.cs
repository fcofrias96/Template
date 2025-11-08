using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Template.Application.Contracts;
using Template.Application.Contracts.Services.Base;
using Template.Application.Contracts.Services.V1;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Security.Contracts.Repositories.V1;
using Template.Security.Contracts.Services.V1;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Services.V1
{
    public class UserServiceAsync : IUserServiceAsync
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IMapper _mapper;
        private readonly IReadServiceAsync<User, SecurityDbContext> _readService;
        private readonly UserManager<User> _userManager;
        private readonly IUserRepositoryAsync _userRepository;
        public UserServiceAsync(
            IUserRepositoryAsync userRepository,
            IMapper mapper,
            IEmailServiceAsync emailService,
            IHttpContextAccessor httpContext,
            IResourceServiceAsync moduleService,
            UserManager<User> userManager,
            IReadServiceAsync<User, SecurityDbContext> readService
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _httpContext = httpContext;
            _userManager = userManager;
            _readService = readService;
        }




        public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await GetUser(userId);
            user.IsDeleted = true;
            await _userManager.UpdateAsync(user);
        }

        public async Task<User> GetByIdAsync<TKey>(TKey id, CancellationToken cancellationToken)
        {
            var parsedId = ParseToString(id);
            return await _readService.GetByIdAsync(parsedId, cancellationToken);
        }
        public async Task<ApiResponse<TDestination>> GetByIdProjectedAsync<TKey, TDestination>(TKey id, CancellationToken cancellationToken)
        {
            var parsedId = ParseToString(id);
            return await _readService.GetByIdProjectedAsync<string, TDestination>(parsedId, cancellationToken);
        }

        public async Task<ApiResponse<IReadOnlyList<TDestination>>> GetProjectedAsync<TDestination>(CancellationToken cancellationToken)
        {
            return await _readService.GetProjectedAsync<TDestination>(cancellationToken);
        }

        public async Task<ApiResponse<PagedCollection<TDestination>>> GetProjectedPaginatedAsync<TDestination, TSpecification>(IQueryParametersBase queryParameters, CancellationToken cancellationToken) where TSpecification : ISpecification<User>
        {
            return await _readService.GetProjectedPaginatedAsync<TDestination, TSpecification>(queryParameters, cancellationToken);
        }

        public async Task<ApiResponse<IReadOnlyList<RoleListDto>>> GetRolesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var parsedId = ParseToString(userId);

            var roles = await _userRepository.GetRolesAsync(parsedId, cancellationToken);
            return new ApiResponse<IReadOnlyList<RoleListDto>>(roles);
        }

        public async Task<User?> GetUserByRefreshToken(string refreshToken, CancellationToken cancellationToken)
        {
            return await _userRepository.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);
        }



        public async Task UpdateAsync(Guid userId, UserUpdateDto userUpdateDTO, CancellationToken cancellationToken)
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
            await _userRepository.UpdateUserRolesAsync(userId.ToString(), userUpdateDTO.Roles, cancellationToken);
        }


        public async Task<UserDetailDto> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserDetailAsync(userId.ToString(), cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }
            return user;
        }


        public async Task<ResourcePermissionDto[]> GetResourcesAsync(string userId, CancellationToken cancellationToken)
        {
            return await _userRepository.GetResourcesAsync(userId, cancellationToken);

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