using Template.Application.Models.DTOs.V1.User;
using Template.Application.Models.Wrappers;
using Template.Identity.Entities;
using Template.Identity.Specification.V1;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IUserServiceAsync
    {
        Task AddToRolesAsync(Guid userId, List<Guid> roles);

        Task AddToRolesAsync(Guid userId, Guid roleId);
        Task DeleteAsync(Guid userId);

        Task<ApiResponse<PagedCollection<UserListDTO>>> GetAllAsync(UserFilterRequest request);

        Task<ApiResponse<UserDetailDTO>> GetByIdAsync(Guid userId);

        Task<ApiResponse<List<string>>> GetRolesAsync(Guid userId);
        Task<User> GetUserByRefreshToken(string refreshToken);


        Task RemoveFromRolesAsync(Guid userId, List<Guid> roles);

        Task RemoveFromRolesAsync(Guid userId, Guid roles);

        Task UpdateAsync(Guid userId, UserUpdateDTO userUpdateDTO);
    }
}