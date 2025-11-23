using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IUserRepositoryAsync : IBaseRepositoryAsync<User>
    {
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

        Task<IReadOnlyList<RoleListDto>> GetRolesAsync(string userId, CancellationToken cancellationToken);

        Task<UserDetailDto?> GetUserDetailAsync(string userId, CancellationToken cancellationToken);

        Task UpdateUserRolesAsync(string userId, string[] newRoleIds, CancellationToken cancellationToken);

        Task<ResourcePermissionDto[]> GetResourcesAsync(string userId, CancellationToken cancellationToken);
    }
}