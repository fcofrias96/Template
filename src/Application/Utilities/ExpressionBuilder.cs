using System.Linq.Expressions;
using System.Reflection;

namespace Template.Application.Utilities
{
    public static class ExpressionBuilder
    {
        public static Expression<Func<TEntity, bool>> BuildContainsOrEqualsExpression<TEntity>(object value, PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");

            var propertyExpression = Expression.Property(parameter, property);

            if (value == null)
            {
                return Expression.Lambda<Func<TEntity, bool>>(Expression.Constant(false), parameter);
            }

            var propertyType = property.PropertyType;

            if (value.GetType().IsArray)
            {
                var array = (Array)value;

                var arrayExpression = Expression.Constant(array);

                var containsMethod = typeof(Enumerable).GetMethods()
                                    .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                                    .MakeGenericMethod(propertyType);

                var containsExpression = Expression.Call(null, containsMethod, arrayExpression, propertyExpression);

                return Expression.Lambda<Func<TEntity, bool>>(containsExpression, parameter);
            }
            else
            {
                if (propertyType == typeof(string) && value is string)
                {
                    var valueExpression = Expression.Constant(value, typeof(string));

                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    var containsExpression = Expression.Call(propertyExpression, containsMethod, valueExpression);

                    return Expression.Lambda<Func<TEntity, bool>>(containsExpression, parameter);
                }
                else
                {
                    var valueExpression = Expression.Constant(value, propertyType);

                    var equalsExpression = Expression.Equal(propertyExpression, valueExpression);

                    return Expression.Lambda<Func<TEntity, bool>>(equalsExpression, parameter);
                }
            }
        }


        public static Expression<Func<TEntity, bool>> BuildRangeExpression<TEntity>(
          object? minValue, object? maxValue, PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");

            var propertyExpression = Expression.Property(parameter, property);

            Expression finalExpression = null;

            var propertyType = property.PropertyType;

            if (minValue != null && maxValue != null)
            {
                var minExpression = Expression.Constant(Convert.ChangeType(minValue, propertyType));
                var maxExpression = Expression.Constant(Convert.ChangeType(maxValue, propertyType));

                var greaterThanOrEqual = Expression.GreaterThanOrEqual(propertyExpression, minExpression);
                var lessThanOrEqual = Expression.LessThanOrEqual(propertyExpression, maxExpression);

                finalExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
            }
            else if (minValue != null)
            {
                var minExpression = Expression.Constant(Convert.ChangeType(minValue, propertyType));
                finalExpression = Expression.GreaterThanOrEqual(propertyExpression, minExpression);
            }
            else if (maxValue != null)
            {
                var maxExpression = Expression.Constant(Convert.ChangeType(maxValue, propertyType));
                finalExpression = Expression.LessThanOrEqual(propertyExpression, maxExpression);
            }
            else
            {
                finalExpression = Expression.Constant(true);
            }

            return Expression.Lambda<Func<TEntity, bool>>(finalExpression, parameter);
        }
    }
}