using System.Reflection;
using System.Text.Json.Serialization;
using Template.Application.Contracts;

namespace Template.Application.Utilities
{
    public abstract class QueryParametersBase<TEntity> : IQueryParametersBase
    {
        public abstract string? SortBy { get; set; }
        public abstract string[] FilterableProperties { get; set; }
        public string? Filter { get; set; }
        public bool Descending { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; } = 30;

        public List<Field> GetFields()
        {

            if (string.IsNullOrEmpty(Filter)) return (List<Field>)Enumerable.Empty<Field>();
            Type type = typeof(TEntity);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => FilterableProperties.Contains(p.Name));



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
                bool range = false;


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
                    range = true;
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
                    Range = range
                };

                fields.Add(field);
            }

            return fields;
        }


    }



  
}