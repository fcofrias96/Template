using Template.Application.Enums;
using Template.Identity.Entities;
using Template.Application.Extensions;

namespace Template.Identity.Seeds
{
    public static class ResourceSeed
    {
        public static List<Resource> Data { get; } = Enum.GetValues(typeof(Resources))
        .Cast<Resources>()
        .Select(Resource => new Resource
        {
            Id = Resource.ToInt(),
            Name = Resource.ToName(),
        })
        .ToList();
    }
}