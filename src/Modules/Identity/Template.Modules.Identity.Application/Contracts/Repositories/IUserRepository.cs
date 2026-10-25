using Template.BuildingBlocks.Application.Contracts.Repositories.Base;
using Template.Modules.Identity.Application.Dtos.Resources;
using Template.Modules.Identity.Application.Dtos.Roles;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Repositories
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

        Task<IReadOnlyList<RoleListDto>> GetRolesAsync(int userId, CancellationToken cancellationToken);

        Task<UserDetailDto?> GetDetailAsync(int userId, CancellationToken cancellationToken);

        Task<ResourcePermissionDto[]> GetResourcesAsync(int userId, CancellationToken cancellationToken);

        Task<int?> GetIdByPublicIdAsync(Guid publicId, CancellationToken cancellationToken);
    }
}
