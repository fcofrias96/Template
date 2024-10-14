using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using Template.Application.Models.DTOs.V1.User;
using Template.Identity.Entities;
using Template.Identity.Specification.V1;

namespace Template.Identity.Contracts.Repositories.V1
{
    public interface IUserRepositoryAsync
    {

        Task AddToRoleAsync(IdentityUserRole<string> userRoles);

        Task AddToRolesAsync(List<IdentityUserRole<string>> identityUserRoles);


        Task<List<UserListDTO>> GetAllProjectedWithPaginationAsync(UserSpecification spec);


        Task<int> GetTotalRecordsAsync(Expression<Func<User, bool>>? criteria);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task RemoveFromRoleAsync(IdentityUserRole<string> userRoles);

        Task RemoveFromRolesAsync(List<IdentityUserRole<string>> identityUserRoles);

    }
}