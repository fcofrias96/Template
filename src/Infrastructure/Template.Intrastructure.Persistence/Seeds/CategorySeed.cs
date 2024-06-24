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
                Name= Categories.Clothes.ToString()
            },
            new Category
            {
                Id= (int) Categories.Electrodomestic,
                Name= Categories.Electrodomestic.ToString()
            },
            new Category
            {
                Id= (int) Categories.Fruit,
                Name= Categories.Fruit.ToString()
            },
            new Category
            {
                Id= (int) Categories.Tool,
                Name= Categories.Tool.ToString()
            }
        };
    }
}