using Category = Template.Core.Domain.Entities.Category;
using Emuns = Template.Core.Domain.Enums;

namespace Template.Infrastructure.Persistence.Seeds
{
    public static class CategorySeed
    {
        public static List<Category> Data { get; } =  new List<Category>
        {
            new Category
            {
                Id= (int) Emuns.Categories.Clothes,
                Name= Emuns.Categories.Clothes.ToString()
            },
            new Category
            {
                Id= (int) Emuns.Categories.Electrodomestic,
                Name= Emuns.Categories.Electrodomestic.ToString()
            },
            new Category
            {
                Id= (int) Emuns.Categories.Fruit,
                Name= Emuns.Categories.Fruit.ToString()
            },
            new Category
            {
                Id= (int) Emuns.Categories.Tool,
                Name= Emuns.Categories.Tool.ToString()
            }
        };
    }
}