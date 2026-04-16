using Template.Application.Contracts.Repositories.Base;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Repositories.V1
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

        Task<IReadOnlyList<RoleListDto>> GetRolesAsync(int userId, CancellationToken cancellationToken);

        Task<UserDetailDto?> GetUserDetailAsync(int userId, CancellationToken cancellationToken);

        Task<ResourcePermissionDto[]> GetResourcesAsync(int userId, CancellationToken cancellationToken);
    }
}