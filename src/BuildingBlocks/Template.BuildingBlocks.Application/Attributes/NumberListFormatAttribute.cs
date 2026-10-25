using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Template.BuildingBlocks.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NumberListFormatAttribute : ValidationAttribute
    {
        private readonly bool _allowDecimal;

        public bool AllowDecimal => _allowDecimal;

        /// <summary>
        // Attribute Constructor.
        /// </summary>
        /// <param name="allowDecimal">Indicates whether decimal numbers are allowed (true) or only integers (false).</param>
        public NumberListFormatAttribute(bool allowDecimal = false)
        {
            _allowDecimal = allowDecimal;
        }

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

                if (_allowDecimal)
                {
                    if (!double.TryParse(trimmedItem, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
                    {
                        return new ValidationResult($"El elemento '{trimmedItem}' no es un número válido.");
                    }
                }
                else
                {
                    if (!int.TryParse(trimmedItem, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                    {
                        return new ValidationResult($"El elemento '{trimmedItem}' no es un número entero válido.");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
