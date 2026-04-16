using Template.Application.Contracts.Services.Base;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Domain.Entities.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IUserService : IReadService<ApplicationUser>
    {
        Task DeleteAsync(int userId, CancellationToken cancellationToken);

        Task<ApiResponse<IReadOnlyList<RoleListDto>>> GetRolesAsync(int userId, CancellationToken cancellationToken);

        Task<ApplicationUser?> GetUserByRefreshToken(string refreshToken, CancellationToken cancellationToken);

        Task UpdateAsync(int userId, UserUpdateDto userUpdateDTO, CancellationToken cancellationToken);

        Task<UserDetailDto> GetUserDetailAsync(int userId, CancellationToken cancellationToken);

        Task<ResourcePermissionDto[]> GetResourcesAsync(int userId, CancellationToken cancellationToken);

        Task UpdateUserRolesAsync(int userId, IEnumerable<int> roleIds, CancellationToken cancellationToken);

    }
}