using System.Globalization;
using System.Linq.Expressions;
using Template.Application.Models.DTOs.V1.Item;
using Template.Application.Utilities;
using Template.Domain.Entities.V1;

namespace Template.Application.Models.Specification.V1
{
    public class ItemSpecification : Specification<Item>
    {
        public ItemSpecification(ItemQueryParameters queryParameters, bool applyPagination) : base(queryParameters, applyPagination)
        {

            var fields = queryParameters.GetField();

            fields.ForEach(field =>
            {

                Expression<Func<Item, bool>> expression;
                var type = field.PropertyInfo.PropertyType;

                if (field.Between)
                {
                    var array = (Array)field.Values;


                    var min = array.GetValue(0);
                    var max = array.GetValue(1);

                    var convetedMin = string.IsNullOrEmpty((string?)min) ? null : ConvertToType(type, min);
                    var convetedMax = string.IsNullOrEmpty((string?)max) ? null : ConvertToType(type, max);

                    if (convetedMax is DateTime dateTime)
                    {
                        convetedMax = dateTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
                    }

                        expression = ExpressionBuilder.BuildRangeExpression<Item>(convetedMin, convetedMax, field.PropertyInfo);
                }
                else
                {
                    var value = ConvertToType(field.PropertyInfo.PropertyType, field.Values);
                    expression = ExpressionBuilder.BuildContainsOrEqualsExpression<Item>(value, field.PropertyInfo);
                }

                CombineCriteria(expression, field.Exclude);

            });
        }


        public static object ConvertToType(Type type, object value)
        {
            if (value == null)
            {
                if (type.IsClass || Nullable.GetUnderlyingType(type) != null)
                {
                    return null;
                }

                return Activator.CreateInstance(type);
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
                else if (type == typeof(DateTime))
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

                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir el valor '{value}' al tipo {type.Name}: {ex.Message}");
                return null;
            }
        }
    }

}
