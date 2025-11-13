using Serilog;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Template.Application.Contracts;
using Template.Application.Exceptions;
using Template.Application.Extensions;
using Template.Application.Utilities;
using Template.Domain.Contracts;

namespace Template.Application.Models.Specification
{
    /// <summary>
    /// Provides a base class for building query specifications for TEntity, including paging, sorting, and criteria.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which the specification is defined.</typeparam>
    public abstract class Specification<TEntity> : ISpecification<TEntity> where TEntity : IEntityBase
    {
        private readonly ILogger _logger;

        protected Specification(IQueryParametersBase queryParametersBase, bool applyPagination, ILogger logger)
        {
            _logger = logger;

            if (applyPagination)
            {
                Skip = queryParametersBase.Offset;
                Take = queryParametersBase.Limit;
            }

            IsDescending = queryParametersBase.IsDescending;
            BuildCriteria(queryParametersBase.BuildFields());
            ApplySortBy(queryParametersBase.SortBy ?? "CreatedAt");
            
        }

        public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }
        public bool IsDescending { get; private set; }

        public Expression<Func<TEntity, object>> SortBy { get; set; }

        public int Skip { get; private set; }
        public int Take { get; private set; }

        // Cache estático por TEntity: propertyPath -> expresión de ordenamiento
        private static readonly ConcurrentDictionary<string, Expression<Func<TEntity, object>>> SortExpressionCache
            = new();

        private static readonly ConcurrentDictionary<Type, TypeConverter?> _typeConverterCache = new();

        private static readonly ConcurrentDictionary<Type, PropertyInfo?> _namePropertyCache = new();

        /// <summary>
        /// Applies a sort by expression to the specification based on the property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to sort by.</param>
        public void ApplySortBy(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name cannot be null or whitespace.", nameof(propertyName));
            }

            // Normaliza y resuelve a ruta final (incluyendo ".Name" si aplica)
            var propertyPath = GetSortPropertyPath(propertyName);

            // Si ya la construimos antes, la reutilizamos
            SortBy = SortExpressionCache.GetOrAdd(propertyPath, value =>
            {
                var parameter = Expression.Parameter(typeof(TEntity), "p");
                var propertyExpression = GetNestedPropertyExpression(parameter, value);
                var conversion = Expression.Convert(propertyExpression, typeof(object));
                return Expression.Lambda<Func<TEntity, object>>(conversion, parameter);
            });

        }

        /// <summary>
        /// Resuelve el "propertyPath" final que se usará para ordenar.
        /// Si la propiedad es una navegación y no se especificó subpropiedad,
        /// por convención usa ".Name" si existe.
        /// </summary>
        private static string GetSortPropertyPath(string propertyName)
        {
            // Si ya viene algo como "Category.Name", no tocamos nada
            if (propertyName.Contains("."))
            {
                return propertyName;
            }

            var directProperty = typeof(TEntity).GetProperty(
                propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (directProperty is null)
            {
                throw new BadRequestException(
                    $"Property '{propertyName}' was not found on type '{typeof(TEntity).Name}'.");
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

        private static Expression GetNestedPropertyExpression(Expression expression, string propertyPath)
        {
            foreach (var member in propertyPath.Split('.'))
            {
                var propertyInfo = expression.Type.GetProperty(
                    member,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo == null)
                {
                    throw new BadRequestException(
                        $"Property '{member}' was not found on type '{expression.Type.Name}'.");
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

        private void BuildCriteria(List<Field> fields)
        {
            fields.ForEach(field =>
            {
                Expression<Func<TEntity, bool>> expression;
                var type = field.PropertyInfo.PropertyType;

                if (field.Range)
                {
                    var array = (Array)field.Values;
                    var min = array.GetValue(0);
                    var max = array.GetValue(1);

                    var convetedMin = string.IsNullOrEmpty((string?)min) ? null : ConvertToType(type, min);
                    var convetedMax = string.IsNullOrEmpty((string?)max) ? null : ConvertToType(type, max);

                    if (convetedMax is DateTimeOffset dateTime)
                    {
                        convetedMax = dateTime.AddDays(1).AddTicks(-1);
                    }

                    expression = ExpressionBuilder.BuildRangeExpression<TEntity>(convetedMin, convetedMax, field.PropertyInfo);
                }
                else
                {
                    var value = ConvertToType(field.PropertyInfo.PropertyType, field.Values);
                    expression = ExpressionBuilder.BuildContainsOrEqualsExpression<TEntity>(value, field.PropertyInfo);
                }

                CombineCriteria(expression, field.Exclude);
            });
        }


        public object? ConvertToType(Type type, object value)
        {
            // Null o vacío
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                // Si es nullable o referencia → null
                if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
                {
                    return null;
                }

                // Value type no nullable → default(T)
                return Activator.CreateInstance(type);
            }

            // Arrays (ej: value = string[] desde querystring)
            if (value is Array valueArray && type != typeof(string))
            {
                var elementType = type.IsArray ? type.GetElementType()! : type;
                var convertedArray = Array.CreateInstance(elementType, valueArray.Length);

                for (var i = 0; i < valueArray.Length; i++)
                {
                    var item = valueArray.GetValue(i);
                    convertedArray.SetValue(ConvertSingleValue(elementType, item), i);
                }

                return convertedArray;
            }

            // Valor simple
            return ConvertSingleValue(type, value);
        }

        private object? ConvertSingleValue(Type type, object? value)
        {
            var targetType = Nullable.GetUnderlyingType(type) ?? type;

            // Si ya es del tipo correcto, devuélvelo
            if (value != null && targetType.IsInstanceOfType(value))
            {
                return value;
            }

            var stringValue = value?.ToString() ?? string.Empty;

            // Tipos básicos con TryParse (evita excepciones costosas)
            if (targetType == typeof(int))
            {
                if (int.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var i))
                    return i;
                return ThrowConversionError(targetType, stringValue);
            }

            if (targetType == typeof(decimal))
            {
                if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    return d;
                return ThrowConversionError(targetType, stringValue);
            }

            if (targetType == typeof(double))
            {
                if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    return d;
                return ThrowConversionError(targetType, stringValue);
            }

            if (targetType == typeof(float))
            {
                if (float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var f))
                    return f;
                return ThrowConversionError(targetType, stringValue);
            }

            if (targetType == typeof(bool))
            {
                if (bool.TryParse(stringValue, out var b))
                    return b;
                return ThrowConversionError(targetType, stringValue);
            }

            if (targetType == typeof(DateTimeOffset))
            {
                if (DateTimeOffset.TryParse(stringValue, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var dto))
                {
                    return dto;
                }

                return ThrowConversionError(targetType, stringValue);
            }

            if (targetType == typeof(DateTime))
            {
                if (DateTime.TryParse(stringValue, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal, out var dt))
                {
                    return dt;
                }

                return ThrowConversionError(targetType, stringValue);
            }

            if (targetType.IsEnum)
            {
                try
                {
                    return Enum.Parse(targetType, stringValue, ignoreCase: true);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error al convertir '{Value}' al enum {TypeName}", stringValue, targetType.Name);
                    throw;
                }
            }

            if (targetType == typeof(string))
            {
                return stringValue;
            }

            // TypeConverter cacheado para tipos complejos
            var converter = _typeConverterCache.GetOrAdd(targetType, t => TypeDescriptor.GetConverter(t));

            if (converter != null && converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    return converter.ConvertFrom(null, CultureInfo.InvariantCulture, stringValue);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error al convertir '{Value}' usando TypeConverter para {TypeName}",
                        stringValue, targetType.Name);
                    throw;
                }
            }

            // Tipos complejos con propiedad "Name"
            if (targetType.IsClass && targetType != typeof(string))
            {
                var nameProperty = _namePropertyCache.GetOrAdd(targetType, t =>
                    t.GetProperty("Name", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                );

                if (nameProperty != null)
                {
                    try
                    {
                        var instance = Activator.CreateInstance(targetType)!;
                        var convertedName = ConvertSingleValue(nameProperty.PropertyType, stringValue);
                        nameProperty.SetValue(instance, convertedName);
                        return instance;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex,
                            "Error al asignar la propiedad 'Name' con valor '{Value}' para el tipo {TypeName}",
                            stringValue, targetType.Name);
                        throw;
                    }
                }
            }

            // Último fallback: Convert.ChangeType (con log si falla)
            try
            {
                return Convert.ChangeType(stringValue, targetType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al convertir el valor '{Value}' al tipo {TypeName}", stringValue, targetType.Name);
                throw;
            }
        }

        private object ThrowConversionError(Type targetType, string value)
        {
            _logger.Error("Error al convertir el valor '{Value}' al tipo {TypeName}", value, targetType.Name);
            // Puedes lanzar tu BadRequestException si quieres algo más de dominio
            throw new BadRequestException($"Value '{value}' is not valid for type '{targetType.Name}'.");
        }
    }
}