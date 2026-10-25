using Template.Modules.Identity.Contracts.Authorization;
using Template.SharedKernel.Domain;

namespace Template.Modules.Identity.Domain.Entities
{
    public class PermissionAction : EntityBase
    {
        public const int Create = PermissionActions.Create;
        public const int Read = PermissionActions.Read;
        public const int Update = PermissionActions.Update;
        public const int Delete = PermissionActions.Delete;
        public const int Export = PermissionActions.Export;
        public string Name { get; set; }
        public ICollection<Permission> Permissions { get; set; } = [];
    }
}
