using Template.Application.Models.DTOs.V1.Role;
using Template.Application.Models.Wrappers;

namespace Template.Identity.Contracts.Services.V1
{
    public interface IRoleServiceAsync
    {
        Task<ApiResponse<List<RoleListDTO>>> GetAllAsync();
    }
}