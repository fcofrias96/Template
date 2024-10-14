using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Template.Application.Exceptions;
using Template.Application.Utilities;
using item = Template.Domain.Entities.V1.Item;

namespace Template.Application.Models.DTOs.V1.Item
{
    public class ItemQueryParameters : QueryParametersBase
    {
        [AllowedValues("Name", "Price", "Description", null, ErrorMessage = "Values allowed are: Name, Price, Description")]
        public override string? SortBy { get; set; }


        private string[] FilterableProporties { get; } = { "CategoryId", "Description", "Price", "Created" };


        public List<Field> GetField()
        {
            

            if(string.IsNullOrEmpty(Filter)) return (List<Field>) Enumerable.Empty<Field>();
            Type type = typeof(item);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance )
                             .Where(p => FilterableProporties.Contains(p.Name));
           


            var filters = Filter.Split(',');
            var fields = new List<Field>();

            foreach (var filter in filters)
            {
                var data = filter.Split(':');
                var propertyName = data[0];
                var property = properties.FirstOrDefault(p => p.Name == propertyName);

                if (property == null)
                {
                    throw new Exception();
                }

                object values;
                bool exclude = false;
                bool between = false;


                if (data[1].StartsWith('!'))
                {
                    var cleanedData = data[1].Substring(2, data[1].Length - 2);
                    values = cleanedData.Split('|');
                    exclude = true;
                }
                else if (data[1].StartsWith('{') && data[1].EndsWith('}'))
                {
                    var cleanedData = data[1].Substring(1, data[1].Length - 2);
                    values = cleanedData.Split('|');
                }
                else if (data[1].StartsWith('[') && data[1].EndsWith(']'))
                {
                    var cleanedData = data[1].Substring(1, data[1].Length - 2);
                    values = cleanedData.Split("..");
                    between = true;
                }
                else
                {
                    values = data[1];
                }


                var field = new Field
                {
                    PropertyInfo = property,
                    Values = values,
                    Exclude = exclude,
                    Between = between
                };

                fields.Add(field);
            }

            return fields;
        }

    }

    public class Field
    {
        public PropertyInfo PropertyInfo { get; set; }
        public object Values { get; set; }
        public bool Exclude { get; set; }
        public bool Between { get; set; }
    }


}