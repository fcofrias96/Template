using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Roles
{
    public record RoleUpdateDto : IHasId, IUpdateMapTo<ApplicationRole>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName
        {
            get
            {
                return Name.ToUpper();
            }
        }
        public int[] Permissions { get; set; }

        public void ApplyTo(ApplicationRole entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.Name = Name;
            entity.NormalizedName = NormalizedName;
        }
    }
}
