using Seemon.Vault.Core.Helpers.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Seemon.Vault.Helpers.Validators
{
    public class SecureStringValidator : ValidationAttribute
    {
        private int _minimumLength;
        private string _compare;
        private string _description;
        private string _compareDescription;

        public SecureStringValidator(int MinimumLength = 5, string Compare = "", string Description = "", string CompareDescription = "")
        {
            _minimumLength = MinimumLength;
            _compare = Compare;
            _description = Description;
            _compareDescription = CompareDescription;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(_description))
            {
                _description = validationContext.DisplayName;
            }
            if (string.IsNullOrEmpty(_compareDescription))
            {
                _compareDescription = _compare;
            }

            if (value is not SecureString secureString)
            {
                throw new ArgumentException("Argument should be of type SecureString.");
            }

            if (secureString.Length < _minimumLength)
            {
                return new ValidationResult($"{_description} must be a minimum of {_minimumLength} characters long.");
            }

            if (!string.IsNullOrEmpty(_compare))
            {
                var compareProperty = validationContext.ObjectInstance.GetType().GetProperty(_compare);
                if(compareProperty is not null)
                {
                    var compare = compareProperty.GetValue(validationContext.ObjectInstance, null) as SecureString;

                    if(compare.ToUnsecuredString() != secureString.ToUnsecuredString())
                    {
                        return new ValidationResult($"{_description} has to be the same as {_compareDescription}");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
