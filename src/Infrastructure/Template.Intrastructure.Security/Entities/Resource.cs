using Template.Domain.Common;

namespace Template.Security.Entities
{
    public class Resource : EntityBase<int>
    {
        public const int User = 1;
        public const int Role = 2;
        public const int ResourceItem = 3;
        public const int Permission = 4;
        public const int Action = 5;
        public const int Configuration = 6;
        public const int Category = 7;
        public const int Item = 8;

        public override int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Permission> Permissions { get; set; } = [];
    }
}