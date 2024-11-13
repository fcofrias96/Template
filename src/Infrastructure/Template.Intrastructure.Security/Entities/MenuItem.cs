using Template.Domain.Common;

namespace Template.Security.Entities
{
    public class MenuItem : EntityBase<int>
    {
        public override int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }

        // Relación jerárquica
        public int? ParentId { get; set; }
        public MenuItem Parent { get; set; }
        public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();

        // Relación con ResourceAction
        public int? ResourceActionId { get; set; }
        public ResourceAction ResourceAction { get; set; }
    }

}