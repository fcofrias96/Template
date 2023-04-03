using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Template.Core.Application.Request;

namespace Template.Core.Application.Filter
{
    public class SortablePropertiesParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var queryParametersType = context?.ParameterInfo?.ParameterType;

            if (parameter.Name.ToLower() == "orderby")
            {
                var properties = queryParametersType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(p => !p.GetGetMethod().IsVirtual);

                var sortableProperties = string.Join(", ", properties.Select(p => p.Name));

                parameter.Description = $"Sortable properties: {sortableProperties}";
            }
        }

       
    }
}