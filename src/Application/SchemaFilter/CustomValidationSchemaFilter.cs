using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Template.Application.Attributes;
using AllowedValuesAttribute = Template.Application.Attributes.AllowedValuesAttribute;

namespace Template.Application.SchemaFilter
{
    public class CustomValidationSchemaFilter : ISchemaFilter
    {
        private static void AppendDescription(IOpenApiSchema schema, string description)
        {
            if (!string.IsNullOrEmpty(schema.Description))
            {
                schema.Description += " " + description;
            }
            else
            {
                schema.Description = description;
            }
        }

        public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.MemberInfo != null)
            {
                var attributes = context.MemberInfo.GetCustomAttributes(true);

                // DateRangeFormatAttribute
                if (attributes.Any(a => a is DateRangeFormatAttribute))
                {
                    var dateRangeDescription = "El formato aceptado es 'fecha', 'fecha..fecha', 'fecha..' o '..fecha' en formato 'yyyy-MM-dd'. Puede anteponer '!' para excluir los registros que coincidan.";
                    AppendDescription(schema, dateRangeDescription);
                }

                // intListFormatAttribute
                if (attributes.Any(a => a is intListFormatAttribute))
                {
                    var intListDescription = "El formato aceptado es una lista de ints separados por comas. Puede anteponer '!' para excluir los registros que coincidan.";
                    AppendDescription(schema, intListDescription);
                }

                // NumberListFormatAttribute
                var numberListAttribute = attributes.OfType<NumberListFormatAttribute>().FirstOrDefault();
                if (numberListAttribute != null)
                {
                    string numberType = numberListAttribute.AllowDecimal ? "números (decimales permitidos)" : "números enteros (solo enteros)";
                    var numberListDescription = $"El formato aceptado es una lista de {numberType} separados por comas. Puede anteponer '!' para excluir los registros que coincidan.";
                    AppendDescription(schema, numberListDescription);
                }

                // NumberRangeFormatAttribute
                if (attributes.Any(a => a is NumberRangeFormatAttribute))
                {
                    var numberRangeDescription = "El formato aceptado es 'número', 'número..número', 'número..' o '..número'. Puede anteponer '!' para excluir los registros que coincidan.";
                    AppendDescription(schema, numberRangeDescription);
                }

                // AllowedValuesAttribute
                var allowedValuesAttribute = attributes.OfType<AllowedValuesAttribute>().FirstOrDefault();
                if (allowedValuesAttribute != null)
                {
                    // IOpenApiSchema.Enum is read-only in Microsoft.OpenApi v1.0+ (JsonNode-based).
                    // Expose the allowed values via the description instead of attempting to assign Enum.
                    var allowed = allowedValuesAttribute.AllowedValues
                        .Where(v => v is not null)
                        .Select(v => v!.ToString())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToArray();

                    if (allowed.Length > 0)
                    {
                        AppendDescription(schema, $"Valores permitidos: {string.Join(", ", allowed)}.");
                    }
                }
            }
        }
    }
}