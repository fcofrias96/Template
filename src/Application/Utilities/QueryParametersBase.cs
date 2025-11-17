using System.Collections.Concurrent;
using System.Reflection;
using Template.Application.Attributes;
using Template.Application.Contracts;

namespace Template.Application.Utilities
{
    public abstract class QueryParametersBase<TEntity> : IQueryParametersBase
    {
        public abstract string? SortBy { get; set; }
        public bool IsDescending { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; } = 30;

        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> QueryPropertiesCache = new();

        private static readonly (Type AttributeType, string Separator, bool IsRange)[] AttributeFormats =
        {
            (typeof(NumberListFormatAttribute), ",", false),
            (typeof(GuidListFormatAttribute), ",", false),
            (typeof(DateRangeFormatAttribute), "..", true),
            (typeof(NumberRangeFormatAttribute), "..", true)
        };

        private static readonly HashSet<string> DefaultProperties = new()
        {
            nameof(SortBy),
            nameof(IsDescending),
            nameof(Offset),
            nameof(Limit)
        };

        public List<Field> BuildFields()
        {
            var fields = new List<Field>();
            var derivedType = GetType();

            // Cache de propiedades del tipo derivado
            var properties = QueryPropertiesCache.GetOrAdd(derivedType, t =>
                t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 .Where(p => !DefaultProperties.Contains(p.Name))
                 .ToArray()
            );

            foreach (var property in properties)
            {
                var propertyValue = GetPropertyValue(property, this);
                if (string.IsNullOrEmpty(propertyValue)) continue;

                bool isExclusion = IsExclusion(ref propertyValue);
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

        private static string? GetPropertyValue(PropertyInfo property, object instance)
        {
            return property.GetValue(instance) as string;
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
                    if (propertyValue.Contains(".."))
                    {
                        isRange = isRangeFlag;
                        return propertyValue.Split(separator);
                    }

                    return propertyValue;
                }
            }
            return propertyValue;
        }

        private static readonly Lazy<Dictionary<string, PropertyInfo>> EntityPropertiesCache =
     new(() => typeof(TEntity)
         .GetProperties(BindingFlags.Public | BindingFlags.Instance)
         .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase));

        private static PropertyInfo GetEntityProperty(string propertyName)
        {
            var map = EntityPropertiesCache.Value;

            if (!map.TryGetValue(propertyName, out var entityProperty))
            {
                throw new InvalidOperationException(
                    $"The class '{typeof(TEntity).Name}' does not contain a property named '{propertyName}'.");
            }

            return entityProperty;
        }
    }
}