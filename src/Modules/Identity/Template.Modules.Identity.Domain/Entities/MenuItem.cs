using Template.SharedKernel.Domain;

namespace Template.Modules.Identity.Domain.Entities
{
    public class MenuItem : EntityBase
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }

        public int? ParentId { get; set; }
        public int Order { get; set; }
        public MenuItem Parent { get; set; }
        public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();

        public int? PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
