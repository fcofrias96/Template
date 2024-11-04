using Template.Application.Contracts.Services.Base;
using Template.Application.Models.DTOs.V1.Roles;
using Template.Application.Models.DTOs.V1.Users;
using Template.Application.Models.Wrappers;
using Template.Identity.DbContexts.V1;
using Template.Identity.Entities;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IUserServiceAsync: IReadServiceAsync<User, IdentityContext>
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