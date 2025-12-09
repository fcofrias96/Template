namespace Template.Security.Dtos.Permissions
{

    namespace Template.Application.Models.DTOs.V1.Roles
    {
        public sealed class UpdateRolePermissionsDto
        {
            public IReadOnlyList<int> Permissions { get; init; } = Array.Empty<int>();
        }
    }

}