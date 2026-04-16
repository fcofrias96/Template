using Template.Domain.Common;

namespace Template.Domain.Entities.V1
{
    public class PermissionAction : EntityBase<int>
    {
        public const int Create = 1;
        public const int Read = 2;
        public const int Update = 3;
        public const int Delete = 4;
        public const int Export = 5;

        public override int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Permission> Permissions { get; set; } = [];
    }
}