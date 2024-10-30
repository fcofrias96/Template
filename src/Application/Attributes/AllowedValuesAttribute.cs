using System.ComponentModel.DataAnnotations;

namespace Template.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class AllowedValuesAttribute : ValidationAttribute
    {
        public object?[] AllowedValues { get; }

        public AllowedValuesAttribute(params object?[] allowedValues)
        {
            AllowedValues = allowedValues;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                if (AllowedValues.Contains(null))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"El valor debe ser uno de los siguientes: {GetAllowedValuesAsString()}.");
                }
            }

            var valueString = value.ToString();

            if (!AllowedValues.Select(v => v?.ToString()).Contains(valueString))
            {
                return new ValidationResult($"El valor debe ser uno de los siguientes: {GetAllowedValuesAsString()}.");
            }

            return ValidationResult.Success;
        }

        private string GetAllowedValuesAsString()
        {
            return string.Join(", ", AllowedValues.Where(v => v is not null).Select(v => v.ToString()));
        }
    }
}