using Template.Application.Contracts.Services.Base;
using Template.Application.Models.DTOs.V1.Resources;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IUserServiceAsync : IReadServiceAsync<User, SecurityDbContext>
    {

        Task DeleteAsync(Guid userId, CancellationToken cancellationToken);

        Task<ApiResponse<IReadOnlyList<RoleListDto>>> GetRolesAsync(Guid userId, CancellationToken cancellationToken);

        Task<User?> GetUserByRefreshToken(string refreshToken, CancellationToken cancellationToken);


        Task UpdateAsync(Guid userId, UserUpdateDto userUpdateDTO, CancellationToken cancellationToken);

        Task<UserDetailDto> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken);

        Task<ResourcePermissionDto[]> GetResourcesAsync(string userId, CancellationToken cancellationToken);
    }
}