using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Seemon.Vault.Helpers.Validators
{
    public class EmailValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            => Regex.IsMatch(value.ToString(), @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")
                ? ValidationResult.Success
                : (new("Invalid email adress. Email address must be in <user@domain.com> format."));
    }
}
