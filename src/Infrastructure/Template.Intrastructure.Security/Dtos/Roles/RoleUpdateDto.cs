using Template.Application.Contracts;

namespace Template.Application.Models.DTOs.V1.Roles
{
    public record RoleUpdateDto: IUpdateDto
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
    }
}