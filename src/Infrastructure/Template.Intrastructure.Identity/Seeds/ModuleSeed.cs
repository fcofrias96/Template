using Template.Identity.Entities;

namespace Template.Identity.Seeds
{
    public static class ModuleSeed
    {
        public static List<Module> Data { get; } = new List<Module>
        {
             new Module
             {
                Id = 1,
                Name = "Users"
             },
             new Module
             {
                 Id = 2,
                Name = "Modules"
             },
             new Module
             {
                Id = 3,
                Name = "Categories"
             },
             new Module
             {
                 Id = 4,
                Name = "Items"
             }
        };
    }
}