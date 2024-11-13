using Template.Application.Contracts.Services.Base;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Security.DbContexts.V1;
using Template.Security.Entities;

namespace Template.Security.Contracts.Services.V1
{
    public interface IUserServiceAsync : IReadServiceAsync<User, SecurityDbContext>
    {
        Task AddToRolesAsync(Guid userId, IReadOnlyList<Guid> roles);

        Task AddToRolesAsync(Guid userId, Guid roleId);

        Task DeleteAsync(Guid userId);

        Task<ApiResponse<IReadOnlyList<RoleListDto>>> GetRolesAsync(Guid userId);

        Task<User> GetUserByRefreshToken(string refreshToken);

        Task RemoveFromRolesAsync(Guid userId, IReadOnlyList<Guid> roles);

        Task RemoveFromRolesAsync(Guid userId, Guid roles);

        Task UpdateAsync(Guid userId, UserUpdateDto userUpdateDTO);

        Task<UserDetailDto> GetUserDetailAsync(Guid userId);
    }
}