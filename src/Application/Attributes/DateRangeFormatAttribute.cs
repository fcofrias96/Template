using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Template.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DateRangeFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is not string strValue)
                return new ValidationResult("El valor debe ser una cadena de texto en formato 'yyyy-MM-dd'.");

            string[] parts = strValue.Split(new[] { ".." }, StringSplitOptions.None);

            if (parts.Length != 2)
                return new ValidationResult("Formato inválido. El formato aceptado es 'fecha..fecha', 'fecha..' o '..fecha' en formato 'yyyy-MM-dd'.");

            bool hasStartDate = !string.IsNullOrWhiteSpace(parts[0]);
            bool hasEndDate = !string.IsNullOrWhiteSpace(parts[1]);

            if (!hasStartDate && !hasEndDate)
                return new ValidationResult("Debe proporcionar al menos una fecha en el formato 'yyyy-MM-dd'.");

            DateTime startDate = default;
            DateTime endDate = default;


            if (hasStartDate && !DateTime.TryParseExact(parts[0], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                return new ValidationResult("La fecha inicial no es válida. El formato aceptado es 'yyyy-MM-dd'.");



            if (hasEndDate && !DateTime.TryParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                return new ValidationResult("La fecha final no es válida. El formato aceptado es 'yyyy-MM-dd'.");


            if (hasStartDate && hasEndDate && startDate > endDate)
                return new ValidationResult("La fecha inicial debe ser anterior o igual a la fecha final.");

            return ValidationResult.Success;
        }
    }
}
