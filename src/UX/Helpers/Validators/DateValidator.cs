using System;
using System.ComponentModel.DataAnnotations;

namespace Seemon.Vault.Helpers.Validators
{
    public class DateValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return value is null
                ? ValidationResult.Success
                : (DateTime)value > DateTime.Now ? ValidationResult.Success : (new("Expiration date has to be in the future."));
        }
    }
}
