using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BibliotecaAPI.Validations
{
    public class PrimeraLetraAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is null || string.IsNullOrEmpty(value.ToString()))
                return ValidationResult.Success;

            char primeraLetra = value.ToString()![0];

            if (!char.IsUpper(primeraLetra))
                return new ValidationResult("La primer letra debe ser mayúscula");

            return ValidationResult.Success;
        }
    }
}
