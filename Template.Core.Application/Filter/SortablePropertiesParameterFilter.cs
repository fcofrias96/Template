using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Template.Core.Application.Filter
{
    public class SortablePropertiesParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var queryParametersType = context?.ParameterInfo?.ParameterType;


            if (parameter.Name.ToLower() == "sortableproperties" && queryParametersType.IsGenericType)
            {
                var entityType = queryParametersType.GetGenericArguments()[0];
                var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(p => !p.GetGetMethod().IsVirtual);
                
                var sortableProperties = string.Join(", ", properties.Select(p => p.Name));

                parameter.Description = $"Sortable properties: {sortableProperties}";
            }
        }
    }
}
