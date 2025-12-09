namespace Template.Security.Dtos.UserRole
{
    public sealed class UpdateUserRolesDto
    {
        public IReadOnlyList<string> RoleIds { get; init; } = Array.Empty<string>();
    }

}