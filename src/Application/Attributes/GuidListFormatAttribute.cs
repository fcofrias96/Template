using System.ComponentModel.DataAnnotations;

namespace Template.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class GuidListFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            if (value is not string strValue)
            {
                return new ValidationResult("El valor debe ser una cadena de texto.");
            }

            strValue = strValue.TrimStart('!');
            var items = strValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < items.Length; i++)
            {
                string trimmedItem = items[i].Trim();

                if (!Guid.TryParse(trimmedItem, out _))
                {
                    return new ValidationResult($"El elemento '{trimmedItem}' no es un GUID válido.");
                }
            }

            return ValidationResult.Success;
        }
    }

}