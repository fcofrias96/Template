using Template.BuildingBlocks.Application.Contracts.Mapping;
using Template.Modules.Identity.Domain.Entities;

namespace Template.Modules.Identity.Application.Dtos.Roles
{
    public class RoleCreateDto : ICreateMapTo<ApplicationRole>
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public string NormalizedName
        {
            get
            {
                return Name.ToUpper();
            }
        }

        public int[] Resources { get; set; }

        public ApplicationRole ToEntity()
        {
            return new ApplicationRole
            {
                Name = Name,
                NormalizedName = NormalizedName
            };
        }
    }
}
