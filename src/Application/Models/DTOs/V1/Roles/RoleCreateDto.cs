namespace Template.Application.Models.DTOs.V1.Roles
{
    public class RoleCreateDto
    {
        public string Name { get; set; }
        public string NormalizedName
        {
            get
            {
                return Name.ToUpper();
            }
        }
    }
}