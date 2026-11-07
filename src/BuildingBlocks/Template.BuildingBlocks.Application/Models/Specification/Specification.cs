using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Template.BuildingBlocks.Application.Contracts;
using Template.BuildingBlocks.Application.Errors;
using Template.BuildingBlocks.Application.Extensions;
using Template.BuildingBlocks.Application.Utilities;
using Template.SharedKernel.Domain;
using Template.SharedKernel.Results;

namespace Template.BuildingBlocks.Application.Models.Specification
{
    /// <summary>
    /// Provides a base class for building query specifications for TEntity, including paging, sorting, and criteria.
    /// Build it with <see cref="Create"/>: invalid client input (unknown sort field, unparsable filter value)
    /// comes back as a failed <see cref="Result"/> instead of an exception.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which the specification is defined.</typeparam>
    public class Specification<TEntity> : ISpecification<TEntity> where TEntity : IEntityBase
    {
        private const string DefaultSortBy = "CreatedAt";

        protected Specification()
        {
        }

        /// <summary>
        /// Builds a specification from the query-string parameters of a request.
        /// </summary>
        public static Result<Specification<TEntity>> Create(IQueryParametersBase queryParametersBase, bool applyPagination = true)
        {
            ArgumentNullException.ThrowIfNull(queryParametersBase);

            var specification = new Specification<TEntity>
            {
                IsDescending = queryParametersBase.IsDescending
            };

            if (applyPagination)
            {
                specification.Skip = queryParametersBase.Offset;
                specification.Take = queryParametersBase.Limit;
            }

            var criteriaResult = specification.BuildCriteria(queryParametersBase.BuildFields());
            if (criteriaResult.IsFailure)
            {
                return criteriaResult.Error;
            }

            var sortResult = specification.ApplySortBy(queryParametersBase.SortBy ?? DefaultSortBy);
            if (sortResult.IsFailure)
            {
                return sortResult.Error;
            }

            return specification;
        }

        public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }

        public bool IsDescending { get; private set; }

        public Expression<Func<TEntity, object>> SortBy { get; set; } = default!;

        public int Skip { get; private set; }

        public int Take { get; private set; }

        // Cache por TEntity: propertyPath -> expresión de ordenamiento
        private static readonly ConcurrentDictionary<string, Expression<Func<TEntity, object>>> SortExpressionCache = new();

        private static readonly ConcurrentDictionary<Type, TypeConverter?> _typeConverterCache = new();

        private static readonly ConcurrentDictionary<Type, PropertyInfo?> _namePropertyCache = new();

        /// <summary>
        /// Applies a sort by expression to the specification based on the property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to sort by.</param>
        public Result ApplySortBy(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return QueryErrors.SortByRequired;
            }

            // Normaliza y resuelve a ruta final (incluyendo ".Name" si aplica)
            var pathResult = GetSortPropertyPath(propertyName);
            if (pathResult.IsFailure)
            {
                return pathResult.Error;
            }

            var propertyPath = pathResult.Value;

            // Si ya la construimos antes, la reutilizamos. Solo se cachean rutas válidas.
            if (!SortExpressionCache.TryGetValue(propertyPath, out var sortExpression))
            {
                var parameter = Expression.Parameter(typeof(TEntity), "p");
                var propertyResult = GetNestedPropertyExpression(parameter, propertyPath);
                if (propertyResult.IsFailure)
                {
                    return propertyResult.Error;
                }

                var conversion = Expression.Convert(propertyResult.Value, typeof(object));
                sortExpression = SortExpressionCache.GetOrAdd(
                    propertyPath,
                    Expression.Lambda<Func<TEntity, object>>(conversion, parameter));
            }

            SortBy = sortExpression;
            return Result.Success();
        }

        /// <summary>
        /// Resuelve el "propertyPath" final que se usará para ordenar.
        /// Si la propiedad es una navegación y no se especificó subpropiedad,
        /// por convención usa ".Name" si existe.
        /// </summary>
        private static Result<string> GetSortPropertyPath(string propertyName)
        {
            // Si ya viene algo como "Category.Name", no tocamos nada (se valida al construir la expresión)
            if (propertyName.Contains('.'))
            {
                return propertyName;
            }

            var directProperty = typeof(TEntity).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (directProperty is null)
            {
                return QueryErrors.UnknownProperty(propertyName, typeof(TEntity).Name);
            }

            // Si es tipo complejo (navegación) y no string, probamos usar Name
            if (directProperty.PropertyType.IsClass &&
                directProperty.PropertyType != typeof(string))
            {
                var defaultProperty = directProperty.PropertyType.GetProperty(
                    "Name",
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (defaultProperty != null)
                {
                    return $"{propertyName}.Name";
                }
            }

            return propertyName;
        }

        private static Result<Expression> GetNestedPropertyExpression(Expression expression, string propertyPath)
        {
            foreach (var member in propertyPath.Split('.'))
            {
                var propertyInfo = expression.Type.GetProperty(
                    member,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    return QueryErrors.UnknownProperty(member, expression.Type.Name);
                }

                expression = Expression.Property(expression, propertyInfo);
            }

            return expression;
        }

        /// <summary>
        /// Updates the current filtering criteria by combining it with a new expression.
        /// If the <paramref name="exclude"/> parameter is set to true, the expression is combined using an AND NOT operation,
        /// excluding items that match the expression. Otherwise, it is combined using an AND operation, including items that match.
        /// </summary>
        /// <param name="expression">The filtering expression to be combined with the existing criteria.</param>
        /// <param name="exclude">If true, applies an AND NOT logic to exclude matching items; otherwise, applies an AND logic.</param>
        private void CombineCriteria(Expression<Func<TEntity, bool>> expression, bool exclude = false)
        {
            if (exclude)
            {
                Criteria = Criteria is null ? expression.Not() : Criteria.AndNot(expression);
            }
            else
            {
                Criteria = Criteria is null ? expression : Criteria.And(expression);
            }
        }

        private Result BuildCriteria(List<Field> fields)
        {
            foreach (var field in fields)
            {
                Expression<Func<TEntity, bool>> expression;
                var type = field.PropertyInfo.PropertyType;

                if (field.Range)
                {
                    var array = (Array)field.Values;
                    var min = array.GetValue(0);
                    var max = array.GetValue(1);

                    var minResult = ConvertBound(type, min);
                    if (minResult.IsFailure)
                    {
                        return minResult.Error;
                    }

                    var maxResult = ConvertBound(type, max);
                    if (maxResult.IsFailure)
                    {
                        return maxResult.Error;
                    }

                    var convertedMin = minResult.Value;
                    var convertedMax = maxResult.Value;

                    if (convertedMax is DateTimeOffset dateTime)
                    {
                        convertedMax = dateTime.AddDays(1).AddTicks(-1);
                    }

                    expression = ExpressionBuilder.BuildRangeExpression<TEntity>(convertedMin, convertedMax, field.PropertyInfo);
                }
                else
                {
                    var valueResult = ConvertToType(field.PropertyInfo.PropertyType, field.Values);
                    if (valueResult.IsFailure)
                    {
                        return valueResult.Error;
                    }

                    expression = ExpressionBuilder.BuildContainsOrEqualsExpression<TEntity>(valueResult.Value!, field.PropertyInfo);
                }

                CombineCriteria(expression, field.Exclude);
            }

            return Result.Success();
        }

        /// <summary>An empty range bound ("..max" or "min..") means "unbounded".</summary>
        private static Result<object?> ConvertBound(Type type, object? bound)
            => string.IsNullOrEmpty((string?)bound) ? Converted(null) : ConvertToType(type, bound);

        private static Result<object?> ConvertToType(Type type, object? value)
        {
            // Null o vacío
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                // Si es nullable o referencia → null
                if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
                {
                    return Converted(null);
                }

                // Value type no nullable → default(T)
                return Converted(Activator.CreateInstance(type));
            }

            // Arrays (ej: value = string[] desde querystring)
            if (value is Array valueArray && type != typeof(string))
            {
                var elementType = type.IsArray ? type.GetElementType()! : type;
                var convertedArray = Array.CreateInstance(elementType, valueArray.Length);

                for (var i = 0; i < valueArray.Length; i++)
                {
                    var itemResult = ConvertSingleValue(elementType, valueArray.GetValue(i));
                    if (itemResult.IsFailure)
                    {
                        return itemResult;
                    }

                    convertedArray.SetValue(itemResult.Value, i);
                }

                return Converted(convertedArray);
            }

            // Valor simple
            return ConvertSingleValue(type, value);
        }

        private static Result<object?> ConvertSingleValue(Type type, object? value)
        {
            var targetType = Nullable.GetUnderlyingType(type) ?? type;

            // Si ya es del tipo correcto, devuélvelo
            if (value != null && targetType.IsInstanceOfType(value))
            {
                return Converted(value);
            }

            var stringValue = value?.ToString() ?? string.Empty;

            // Tipos básicos con TryParse (evita excepciones costosas)
            if (targetType == typeof(int))
            {
                return int.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var i)
                    ? Converted(i)
                    : InvalidValue(targetType, stringValue);
            }

            if (targetType == typeof(decimal))
            {
                return decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
                    ? Converted(d)
                    : InvalidValue(targetType, stringValue);
            }

            if (targetType == typeof(double))
            {
                return double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
                    ? Converted(d)
                    : InvalidValue(targetType, stringValue);
            }

            if (targetType == typeof(float))
            {
                return float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var f)
                    ? Converted(f)
                    : InvalidValue(targetType, stringValue);
            }

            if (targetType == typeof(bool))
            {
                return bool.TryParse(stringValue, out var b)
                    ? Converted(b)
                    : InvalidValue(targetType, stringValue);
            }

            if (targetType == typeof(DateTimeOffset))
            {
                return DateTimeOffset.TryParse(
                        stringValue,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var dto)
                    ? Converted(dto)
                    : InvalidValue(targetType, stringValue);
            }

            if (targetType == typeof(DateTime))
            {
                return DateTime.TryParse(
                        stringValue,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal,
                        out var dt)
                    ? Converted(dt)
                    : InvalidValue(targetType, stringValue);
            }

            if (targetType.IsEnum)
            {
                return Enum.TryParse(targetType, stringValue, ignoreCase: true, out var enumValue)
                    ? Converted(enumValue)
                    : InvalidValue(targetType, stringValue);
            }

            if (targetType == typeof(string))
            {
                return Converted(stringValue);
            }

            // Las conversiones siguientes solo exponen APIs que lanzan excepciones:
            // se capturan aquí y se devuelven como error de validación.

            // TypeConverter cacheado para tipos complejos
            var converter = _typeConverterCache.GetOrAdd(targetType, t => TypeDescriptor.GetConverter(t));

            if (converter != null && converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    return Converted(converter.ConvertFrom(null, CultureInfo.InvariantCulture, stringValue));
                }
                catch (Exception)
                {
                    return InvalidValue(targetType, stringValue);
                }
            }

            // Tipos complejos con propiedad "Name"
            if (targetType.IsClass && targetType != typeof(string))
            {
                var nameProperty = _namePropertyCache.GetOrAdd(
                    targetType,
                    t => t.GetProperty("Name", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance));

                if (nameProperty != null)
                {
                    var nameResult = ConvertSingleValue(nameProperty.PropertyType, stringValue);
                    if (nameResult.IsFailure)
                    {
                        return InvalidValue(targetType, stringValue);
                    }

                    try
                    {
                        var instance = Activator.CreateInstance(targetType)!;
                        nameProperty.SetValue(instance, nameResult.Value);
                        return Converted(instance);
                    }
                    catch (Exception)
                    {
                        return InvalidValue(targetType, stringValue);
                    }
                }
            }

            // Último fallback: Convert.ChangeType
            try
            {
                return Converted(Convert.ChangeType(stringValue, targetType, CultureInfo.InvariantCulture));
            }
            catch (Exception)
            {
                return InvalidValue(targetType, stringValue);
            }
        }

        private static Result<object?> Converted(object? value) => Result.Success(value);

        private static Result<object?> InvalidValue(Type targetType, string value)
            => Result.Failure<object?>(QueryErrors.InvalidValue(value, targetType.Name));
    }
}
