using Template.Application.Enums;
using Category = Template.Domain.Entities.V1.Category;

namespace Template.Persistence.Seeds
{
    public static class CategorySeed
    {
        public static List<Category> Data { get; } = new List<Category>
        {
            new Category
            {
                Id= (int) Categories.Clothes,
                Name= nameof(Categories.Clothes)
            },
            new Category
            {
                Id= (int) Categories.Electrodomestic,
                Name= nameof(Categories.Electrodomestic)
            },
            new Category
            {
                Id= (int) Categories.Fruit,
                Name= nameof(Categories.Fruit)
            },
            new Category
            {
                Id= (int) Categories.Tool,
                Name= nameof(Categories.Tool)
            }
        };
    }
}