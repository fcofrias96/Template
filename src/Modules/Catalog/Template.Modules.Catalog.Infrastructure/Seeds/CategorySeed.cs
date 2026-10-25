
using Template.Modules.Catalog.Domain.Entities;

namespace Template.Modules.Catalog.Infrastructure.Seeds
{
    public static class CategorySeed
    {
        public static List<Category> Data { get; } = new List<Category>
        {
            new Category
            {
                Id=  Category.Clothes,
                Name= nameof(Category.Clothes)
            },
            new Category
            {
                Id=  Category.HouseholdAppliance,
                Name= nameof(Category.HouseholdAppliance)
            },
            new Category
            {
                Id=  Category.Fruit,
                Name= nameof(Category.Fruit)
            },
            new Category
            {
                Id=  Category.Tool,
                Name= nameof(Category.Tool)
            }
        };
    }
}
