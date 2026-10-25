using Template.SharedKernel.Domain;

namespace Template.Modules.Catalog.Domain.Entities
{
    public class Category : EntityBase
    {
        public const int Clothes = 1;
        public const int HouseholdAppliance = 2;
        public const int Fruit = 3;
        public const int Tool = 4;
        public string Name { get; set; }
        public virtual IReadOnlyCollection<Item> Items { get; set; }
    }
}
