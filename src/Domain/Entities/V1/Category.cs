using Template.Domain.Common;

namespace Template.Domain.Entities.V1
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