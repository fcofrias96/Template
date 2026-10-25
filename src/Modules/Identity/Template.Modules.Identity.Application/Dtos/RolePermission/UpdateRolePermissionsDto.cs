namespace Template.Modules.Identity.Application.Dtos.RolePermission
{
    public sealed class UpdateRolePermissionsDto
    {
        public IReadOnlyList<int> Permissions { get; init; } = Array.Empty<int>();
    }
}
