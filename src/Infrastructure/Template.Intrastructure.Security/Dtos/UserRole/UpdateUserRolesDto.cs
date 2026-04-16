namespace Template.Security.Dtos.UserRole
{
    public sealed class UpdateUserRolesDto
    {
        public IReadOnlyList<int> RoleIds { get; init; } = Array.Empty<int>();
    }

}