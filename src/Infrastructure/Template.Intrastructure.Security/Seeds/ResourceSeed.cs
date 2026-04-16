using System.Collections.Generic;
using Template.Domain.Entities.V1;

namespace Template.Security.Seeds
{
    public static class ResourceSeed
    {
        public static List<Resource> Data { get; } = new List<Resource>
        {
            new Resource { Id = Resource.User, Name = nameof(Resource.User) },
            new Resource { Id = Resource.Role, Name = nameof(Resource.Role) },
            new Resource { Id = Resource.ResourceItem, Name = nameof(Resource.ResourceItem) },
            new Resource { Id = Resource.Permission, Name = nameof(Resource.Permission) },
            new Resource { Id = Resource.Action, Name = nameof(Resource.Action) },
            new Resource { Id = Resource.Configuration, Name = nameof(Resource.Configuration) },
            new Resource { Id = Resource.Category, Name = nameof(Resource.Category) },
            new Resource { Id = Resource.Item, Name = nameof(Resource.Item) },
        };
    }
}