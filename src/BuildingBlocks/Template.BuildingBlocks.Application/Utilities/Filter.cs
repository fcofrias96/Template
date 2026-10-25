using System.Reflection;

namespace Template.BuildingBlocks.Application.Utilities
{
    public class Field
    {
        public PropertyInfo PropertyInfo { get; set; }
        public object Values { get; set; }
        public bool Exclude { get; set; }
        public bool Range { get; set; }
    }
}
