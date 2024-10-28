using System.Reflection;
using Template.Application.Attributes;
using Template.Application.Contracts;

namespace Template.Application.Utilities
{
    public abstract class QueryParametersBase<TEntity> : IQueryParametersBase
    {
        public abstract string? SortBy { get; set; }
        public bool IsDescending { get; set; }
        public int Offset { get;  }
        public int Limit { get; } = 30;

        private readonly (Type AttributeType, string Separator, bool IsRange)[] AttributeFormats = 
        {
            (typeof(NumberListFormatAttribute), ",", false),
            (typeof(GuidListFormatAttribute), ",", false),
            (typeof(DateRangeFormatAttribute), "..", true),
            (typeof(NumberRangeFormatAttribute), "..", true)
        };

        private readonly string[] DefaultProperties =
        {
            nameof(SortBy),
            nameof(IsDescending),
            nameof(Offset),
            nameof(Limit)
        };

        public List<Field> BuildFields()
        {
            var fields = new List<Field>();
            var derivedType = this.GetType();

            var properties = derivedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                        .Where(p => !DefaultProperties.Contains(p.Name));

            foreach (var property in properties)
            {
                var propertyValue = GetPropertyValue(property);
                if (string.IsNullOrEmpty(propertyValue)) continue;

                bool isExclusion =  IsExclusion(ref propertyValue);
                bool isRange = false;
                object value = ProcessValue(property, propertyValue, ref isRange);

                var entityProperty = GetEntityProperty(property.Name);
                var field = new Field
                {
                    PropertyInfo = entityProperty,
                    Values = value,
                    Exclude = isExclusion,
                    Range = isRange
                };

                fields.Add(field);
            }

            return fields;
        }

        private string? GetPropertyValue(PropertyInfo property)
        {
            return property.GetValue(this)?.ToString();
        }

        private static bool IsExclusion(ref string propertyValue)
        {
            if (propertyValue.StartsWith('!'))
            {
                propertyValue = propertyValue.Substring(1);
                return true;
            }
            return false;
        }

        private object ProcessValue(PropertyInfo property, string propertyValue, ref bool isRange)
        {
            foreach (var (attributeType, separator, isRangeFlag) in AttributeFormats)
            {
                if (property.IsDefined(attributeType))
                {
                    isRange = isRangeFlag;
                    return propertyValue.Split(separator);
                }
            }
            return propertyValue;
        }

        private static PropertyInfo GetEntityProperty(string propertyName)
        {
            var entityProperty = typeof(TEntity).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (entityProperty == null)
            {
                throw new Exception($"The class '{typeof(TEntity).Name}' does not contain a property named '{propertyName}'.");
            }
            return entityProperty;
        }
    }

}