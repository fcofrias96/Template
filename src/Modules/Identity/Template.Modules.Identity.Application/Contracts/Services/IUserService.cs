using Template.BuildingBlocks.Application.Contracts.Services.Base;
using Template.Modules.Identity.Application.Dtos.Resources;
using Template.Modules.Identity.Application.Dtos.Roles;
using Template.Modules.Identity.Application.Dtos.Users;
using Template.BuildingBlocks.Application.Models.Wrappers;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Contracts.Services
{
    public interface IUserService : IReadService<ApplicationUser>
    {
        Task DeleteAsync(int userId, CancellationToken cancellationToken);

        Task<ApiResponse<IReadOnlyList<RoleListDto>>> GetRolesAsync(int userId, CancellationToken cancellationToken);

        Task<ApplicationUser?> GetByRefreshToken(string refreshToken, CancellationToken cancellationToken);

        Task UpdateAsync(int userId, UserUpdateDto userUpdateDTO, CancellationToken cancellationToken);

        Task<UserDetailDto> GetDetailAsync(int userId, CancellationToken cancellationToken);

        Task<ResourcePermissionDto[]> GetResourcesAsync(int userId, CancellationToken cancellationToken);

        Task UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, CancellationToken cancellationToken);

    }
}
