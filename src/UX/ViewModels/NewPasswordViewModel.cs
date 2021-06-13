using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Helpers.Validators;
using Seemon.Vault.Helpers.ViewModels;
using System.Security;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class NewPasswordViewModel : ViewModelBase
    {
        private readonly IWindowManagerService _windowManagerService;

        private SecureString _password;
        private SecureString _confirmPassword;

        private ICommand _submitCommand;

        public NewPasswordViewModel(IWindowManagerService windowManagerService) => _windowManagerService = windowManagerService;

        public ICommand SubmitCommand => _submitCommand ??= RegisterCommand(OnSubmit, CanSubmit);

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

        private bool CanSubmit() => !HasErrors && Password is not null && ConfirmPassword is not null;

        private void OnSubmit() => _windowManagerService.GetWindow(PageKey)?.CloseDialog(true);
    }
}
