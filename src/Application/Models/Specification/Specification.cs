using Serilog;
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
            if (applyPagination)
            {
                Skip = queryParametersBase.Offset;
                Take = queryParametersBase.Limit;
            }

            IsDescending = queryParametersBase.IsDescending;
            BuildCriteria(queryParametersBase.BuildFields());
            ApplySortBy(queryParametersBase.SortBy ?? "Created");
            _logger = logger;
        }

        public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }
        public bool IsDescending { get; private set; }

        public Expression<Func<TEntity, object>> SortBy { get; set; }

        public int Skip { get; private set; }
        public int Take { get; private set; }

        /// <summary>
        /// Applies an sort by expression to the specification based on the property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to sort by.</param>
        public void ApplySortBy(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name cannot be null or whitespace.", nameof(propertyName));
            }

            // Determine the property path to use.
            // If the property is a complex type (navigation property) and no nested property is specified,
            // use the "Name" property by default if it exists.
            var propertyPath = propertyName;
            var directProperty = typeof(TEntity).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (directProperty != null && !propertyName.Contains("."))
            {
                if (directProperty.PropertyType.IsClass && directProperty.PropertyType != typeof(string))
                {
                    var defaultProperty = directProperty.PropertyType.GetProperty("Name", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (defaultProperty != null)
                    {
                        propertyPath = $"{propertyName}.Name";
                    }
                }
            }

            var parameter = Expression.Parameter(typeof(TEntity), "p");
            var propertyExpression = GetNestedPropertyExpression(parameter, propertyPath);
            var conversion = Expression.Convert(propertyExpression, typeof(object));
            var lambda = Expression.Lambda<Func<TEntity, object>>(conversion, parameter);

            SortBy = lambda;
        }

     
        private static Expression GetNestedPropertyExpression(Expression expression, string propertyPath)
        {
            foreach (var member in propertyPath.Split('.'))
            {
                var propertyInfo = expression.Type.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    throw new BadRequestException($"Property '{member}' was not found on type '{expression.Type.Name}'.");
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
                        convetedMax = dateTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
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
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }

            if (value.GetType().IsArray)
            {
                var elementType = type.IsArray ? type.GetElementType() : type;

                var valueArray = (Array)value; var convertedArray = Array.CreateInstance(elementType, valueArray.Length);

                for (int i = 0; i < valueArray.Length; i++)
                {
                    var convertedValue = Convert.ChangeType(valueArray.GetValue(i), elementType, CultureInfo.InvariantCulture);
                    convertedArray.SetValue(convertedValue, i);
                }

                return convertedArray;
            }

            try
            {
                if (Nullable.GetUnderlyingType(type) != null)
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                if (type == typeof(int))
                {
                    return Convert.ToInt32(value, CultureInfo.InvariantCulture);
                }
                else if (type == typeof(decimal))
                {
                    return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                }
                else if (type == typeof(DateTimeOffset))
                {
                    return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
                }
                else if (type == typeof(double))
                {
                    return Convert.ToDouble(value, CultureInfo.InvariantCulture);
                }
                else if (type == typeof(float))
                {
                    return Convert.ToSingle(value, CultureInfo.InvariantCulture);
                }
                else if (type == typeof(bool))
                {
                    return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                }

                // Intentar usar TypeConverter para tipos complejos
                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null && converter.CanConvertFrom(value.GetType()))
                {
                    return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                }

                // Si es una clase compleja (no primitiva ni string), intenta asignar la propiedad "Name"
                if (type.IsClass && type != typeof(string))
                {
                    var defaultProperty = type.GetProperty("Name", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (defaultProperty != null)
                    {
                        var instance = Activator.CreateInstance(type);
                        var convertedValue = Convert.ChangeType(value, defaultProperty.PropertyType, CultureInfo.InvariantCulture);
                        defaultProperty.SetValue(instance, convertedValue);
                        return instance;
                    }
                }


                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                _logger.Error("Error al convertir el valor '{value}' al tipo {type.Name}: {ex.Message}", value, type.Name, ex.Message);
                throw;
            }
        }
    }
}