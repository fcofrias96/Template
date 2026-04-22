using Template.Application.Contracts;

namespace Template.Application.Models.DTOs.V1.Resources
{
    public class ResourceUpdateDto: IUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}