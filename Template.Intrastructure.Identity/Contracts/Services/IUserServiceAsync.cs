using System.Security.Claims;
using Template.Core.Application.DTOs.User;
using Template.Core.Domain.Entities;
using Template.Infrastructure.Identity.Entity;

namespace Template.Infrastructure.Identity.Contracts.Services
{
    public interface IUserServiceAsync
    {
        Task<string> AddAsync(UserCreateDTO userCreateDTO);

        Task AddToRolesAsync(Guid id, List<string> roles);

        Task<IList<string>> GetRolesAsync(Guid id);

        Task RemoveFromRolesAsync(Guid id, List<string> roles);

        Task DeleteAsync(Guid Id);

        Task<bool> ExistsAsync(Guid Id);

        Task<PagedResult<UserListDTO>> GetAllAsync(QueryParameters<User> queryParameters);

        Task<UserDTO> GetByIdAsync(Guid id);

        Task UpdateAsync(Guid id, UserUpdateDTO source);

        Task AddPermissionsAsync(Guid userId, List<string> permissions);

        Task<IList<Claim>> GetPermissionsAsync(Guid id);
    }
}