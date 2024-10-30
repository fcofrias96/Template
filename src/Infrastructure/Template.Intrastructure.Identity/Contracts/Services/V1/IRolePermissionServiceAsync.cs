namespace Template.Identity.Contracts.Services.V1
{
    public interface IRolePermissionServiceAsync
    {
        Task AddPermissionToRoleAsync(Guid roleId, int[] permissionsId);

        Task RemovePermissionToRoleAsync(Guid roleId, int[] permissionsId);
    }
}