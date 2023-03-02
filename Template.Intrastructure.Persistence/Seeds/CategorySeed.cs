using Template.Core.Domain.Entities;
using Emuns = Template.Core.Application.Emuns;

namespace Template.Infrastructure.Persistence.Seeds
{
    public class CategorySeed
    {
        public static List<Category> data = new List<Category>
        {
            new Category
            {
                Id= (int) Emuns.Category.Clothes,
                Name= Emuns.Category.Clothes.ToString()
            },
            new Category
            {
                Id= (int) Emuns.Category.Electrodomestic,
                Name= Emuns.Category.Electrodomestic.ToString()
            },
            new Category
            {
                Id= (int) Emuns.Category.Fruit,
                Name= Emuns.Category.Fruit.ToString()
            },
            new Category
            {
                Id= (int) Emuns.Category.Tool,
                Name= Emuns.Category.Tool.ToString()
            }
        };
    }
}