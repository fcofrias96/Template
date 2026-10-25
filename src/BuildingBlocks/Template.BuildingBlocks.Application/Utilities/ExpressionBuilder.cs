using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Template.BuildingBlocks.Application.Utilities
{
    public static class ExpressionBuilder
    {
        private static readonly MethodInfo s_stringContains =
            typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        private static readonly MethodInfo s_enumerableContainsDef =
            typeof(Enumerable).GetMethods()
                .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);

        // Cachea la "propiedad por defecto" para tipos complejos (p.ej. Name)
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> s_defaultStringPropCache = new();

        public static Expression<Func<TEntity, bool>> BuildContainsOrEqualsExpression<TEntity>(object value, PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var propertyExpr = Expression.Property(parameter, property);

            if (value is null)
                return Expression.Lambda<Func<TEntity, bool>>(Expression.Constant(false), parameter);

            var propertyType = property.PropertyType;

            // 1) value es array -> x => array.Contains(x.Prop)
            if (value.GetType().IsArray)
            {
                var array = (Array)value;

                if (array.Length == 0)
                    return Expression.Lambda<Func<TEntity, bool>>(Expression.Constant(false), parameter);

                var elementType = propertyType; // el tipo del campo
                var typedArray = EnsureTypedArray(array, elementType);

                var arrayConst = Expression.Constant(typedArray, elementType.MakeArrayType());
                var contains = s_enumerableContainsDef.MakeGenericMethod(elementType);
                var call = Expression.Call(null, contains, arrayConst, propertyExpr);
                return Expression.Lambda<Func<TEntity, bool>>(call, parameter);
            }

            // 2) string -> x => x.Prop.Contains(value)
            if (propertyType == typeof(string) && value is string s)
            {
                var valueExpr = Expression.Constant(s, typeof(string));
                var call = Expression.Call(propertyExpr, s_stringContains, valueExpr);
                return Expression.Lambda<Func<TEntity, bool>>(call, parameter);
            }

            // 3) fechas -> x => x.Prop between [startOfDay, endOfDay]
            if (IsDateLike(propertyType))
            {
                GetDayBounds(value, out var start, out var end);
                return BuildRangeExpression<TEntity>(start, end, property);
            }

            // 4) tipos complejos con prop "Name" string (cacheado)
            if (propertyType.IsClass && propertyType != typeof(string))
            {
                var nameProp = s_defaultStringPropCache.GetOrAdd(
                    propertyType,
                    t => t.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                          ?.PropertyType == typeof(string)
                          ? t.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                          : null);

                if (nameProp != null)
                {
                    var nested = Expression.Property(propertyExpr, nameProp);
                    var strVal = nameProp.GetValue(value) as string ?? value.ToString() ?? string.Empty;
                    var valExpr = Expression.Constant(strVal, typeof(string));
                    var call = Expression.Call(nested, s_stringContains, valExpr);
                    return Expression.Lambda<Func<TEntity, bool>>(call, parameter);
                }
            }

            // 5) igual estricto con constante del tipo exacto (coerce seguro)
            var coerced = CoerceToType(value, propertyType);
            var constExpr = Expression.Constant(coerced, propertyType);
            var eq = Expression.Equal(propertyExpr, constExpr);
            return Expression.Lambda<Func<TEntity, bool>>(eq, parameter);
        }

        public static Expression<Func<TEntity, bool>> BuildRangeExpression<TEntity>(
            object? minValue, object? maxValue, PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var propertyExpr = Expression.Property(parameter, property);
            var propertyType = property.PropertyType;

            Expression body;

            if (minValue is not null && maxValue is not null)
            {
                var minConst = Expression.Constant(CoerceToType(minValue, propertyType), propertyType);
                var maxConst = Expression.Constant(CoerceToType(maxValue, propertyType), propertyType);
                var ge = Expression.GreaterThanOrEqual(propertyExpr, minConst);
                var le = Expression.LessThanOrEqual(propertyExpr, maxConst);
                body = Expression.AndAlso(ge, le);
            }
            else if (minValue is not null)
            {
                var minConst = Expression.Constant(CoerceToType(minValue, propertyType), propertyType);
                body = Expression.GreaterThanOrEqual(propertyExpr, minConst);
            }
            else if (maxValue is not null)
            {
                var maxConst = Expression.Constant(CoerceToType(maxValue, propertyType), propertyType);
                body = Expression.LessThanOrEqual(propertyExpr, maxConst);
            }
            else
            {
                body = Expression.Constant(true);
            }

            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }

        // ==== Helpers ====

        private static bool IsDateLike(Type t)
        {
            var nt = Nullable.GetUnderlyingType(t) ?? t;
            return nt == typeof(DateTime) || nt == typeof(DateTimeOffset);
        }

        private static void GetDayBounds(object value, out object start, out object end)
        {
            if (value is DateTimeOffset dto)
            {
                // Mantén el offset original
                var s = new DateTimeOffset(dto.Date, dto.Offset);
                start = s;
                end = s.AddDays(1).AddTicks(-1);
                return;
            }
            if (value is DateTime dt)
            {
                var s = dt.Date;
                start = s;
                end = s.AddDays(1).AddTicks(-1);
                return;
            }
            throw new ArgumentException("Value must be DateTime or DateTimeOffset for date range filtering.");
        }

        private static Array EnsureTypedArray(Array source, Type elementType)
        {
            var srcElem = source.GetType().GetElementType()!;
            if (srcElem == elementType) return source;

            var arr = Array.CreateInstance(elementType, source.Length);
            for (int i = 0; i < source.Length; i++)
                arr.SetValue(CoerceToType(source.GetValue(i), elementType), i);
            return arr;
        }

        private static object? CoerceToType(object? value, Type targetType)
        {
            if (value is null) return null;

            var nonNullable = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (nonNullable.IsEnum)
            {
                // Soporta int/string -> enum
                return value is string s
                    ? Enum.Parse(nonNullable, s, ignoreCase: true)
                    : Enum.ToObject(nonNullable, value);
            }

            if (nonNullable == typeof(DateTimeOffset))
            {
                if (value is DateTimeOffset dto) return WrapNullable(dto, targetType);
                if (value is DateTime dt) return WrapNullable(new DateTimeOffset(dt), targetType);
                // evita Convert.ChangeType para DTO
            }

            if (nonNullable == typeof(int))
            {
                if (value is int g) return WrapNullable(g, targetType);
                if (value is string sg && int.TryParse(sg, out var parsed)) return WrapNullable(parsed, targetType);
            }

            // Para DateTime/numéricos/strings usa Convert con cultura invariante
            var converted = System.Convert.ChangeType(value, nonNullable, CultureInfo.InvariantCulture);
            return WrapNullable(converted, targetType);

            static object? WrapNullable(object v, Type t) =>
                Nullable.GetUnderlyingType(t) is null ? v : v;
        }
    }
}
