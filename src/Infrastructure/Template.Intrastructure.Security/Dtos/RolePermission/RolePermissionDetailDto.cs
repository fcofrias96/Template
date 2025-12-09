namespace Template.Security.Dtos.RolePermission
{
    public sealed class RolePermissionDetailDto
    {
        public int PermissionId { get; init; }
        public string PermissionName { get; init; }

        public int ResourceId { get; init; }
        public string ResourceName { get; init; }
        public int ActionId { get; init; }
        public string ActionName { get; init; }
    }


}