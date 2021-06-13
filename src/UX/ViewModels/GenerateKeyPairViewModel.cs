using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Helpers.Validators;
using Seemon.Vault.Helpers.ViewModels;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class GenerateKeyPairViewModel : ViewModelBase
    {
        public enum KeySizes
        {
            [Description("1024 - Not recommended. Good for testing purposes.")]
            KEY1024 = 1024,
            [Description("2048 - Minimum recommended")]
            KEY2048 = 2048,
            [Description("3072 - Secure")]
            KEY3072 = 3072,
            [Description("4096 - Most Secure")]
            KEY4096 = 4096
        }

        private readonly IWindowManagerService _windowManagerService;

        private string _name;
        private string _email;
        private string _comment;
        private KeySizes _keySize = KeySizes.KEY2048;
        private SecureString _password;
        private SecureString _confirmPassword;
        private DateTime? _expiry;

        private ICommand _submitCommand;

        public GenerateKeyPairViewModel(IWindowManagerService windowManagerService) => _windowManagerService = windowManagerService;

        public ICommand SubmitCommand => _submitCommand ??= RegisterCommand(OnSubmit, CanSubmit);

        [Required]
        [MinLength(5, ErrorMessage = "Name has to be a minimum of 5 characters long.")]
        public string Name
        {
            get => _name; set => SetProperty(ref _name, value, true);
        }

        [Required]
        [EmailValidator]
        public string Email
        {
            get => _email; set => SetProperty(ref _email, value, true);
        }

        public string Comment
        {
            get => _comment; set => SetProperty(ref _comment, value);
        }

        public KeySizes KeySize
        {
            get => _keySize; set => SetProperty(ref _keySize, value);
        }

        [SecureStringValidator]
        public SecureString Password
        {
            get => _password; set => SetProperty(ref _password, value, true);
        }

        [SecureStringValidator(Compare: nameof(Password), Description: "Confirmation Password")]
        public SecureString ConfirmPassword
        {
            get => _confirmPassword; set => SetProperty(ref _confirmPassword, value, true);
        }

        [DateValidator]
        public DateTime? Expiry
        {
            get => _expiry; set => SetProperty(ref _expiry, value, true);
        }

        private bool CanSubmit() => !HasErrors && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email) && Password is not null && ConfirmPassword is not null;

        private void OnSubmit() => _windowManagerService.GetWindow(PageKey)?.CloseDialog(true);
    }
}
