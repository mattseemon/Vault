using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Seemon.Vault.Helpers.Validators
{
    public class PathValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return Directory.Exists(value.ToString()) ? ValidationResult.Success : (new("Location has to be a valid path."));
        }
    }
}
