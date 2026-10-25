namespace Template.Application.Models.DTOs.V1.Resources
{
    public class ResourceDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string[] Permissions { get; set; }
    }
}