using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Template.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NumberRangeFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is not string strValue)
            {
                return new ValidationResult("El valor debe ser una cadena de texto.");
            }

            string[] parts = strValue.Split(new string[] { ".." }, StringSplitOptions.None);

            if (parts.Length != 2)
            {
                return new ValidationResult("El formato debe ser 'número..número', 'número..' o '..número'.");
            }

            double num1 = 0, num2 = 0;
            bool hasNum1 = !string.IsNullOrWhiteSpace(parts[0]);
            bool hasNum2 = !string.IsNullOrWhiteSpace(parts[1]);

            if (!hasNum1 && !hasNum2)
            {
                return new ValidationResult("Formato inválido. Debe proporcionar al menos un número.");
            }


            if (hasNum1 && !double.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out num1))
            {
                return new ValidationResult($"El primer número '{parts[0]}' no es válido.");
            }


            if (hasNum2 && !double.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out num2))
            {
                return new ValidationResult($"El segundo número '{parts[1]}' no es válido.");
            }


            if (hasNum1 && hasNum2 && num1 > num2)
            {
                return new ValidationResult($"El primer número ({num1}) debe ser menor o igual al segundo número ({num2}).");
            }


            return ValidationResult.Success;
        }
    }
}
