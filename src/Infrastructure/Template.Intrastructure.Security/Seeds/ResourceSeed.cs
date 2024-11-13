using Template.Application.Enums;
using Template.Application.Extensions;
using Template.Security.Entities;

namespace Template.Security.Seeds
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