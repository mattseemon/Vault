using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models.Settings;
using Seemon.Vault.Helpers.Validators;
using Seemon.Vault.Helpers.ViewModels;
using Seemon.Vault.Models.Views;
using System.Security;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class PasswordViewModel : ViewModelBase
    {
        private readonly IWindowManagerService _windowManagerService;
        private readonly ISettingsService _settingsService;

        private SecureString _password;
        private ICommand _submitCommand;

        private string _alert;

        public PasswordViewModel(IWindowManagerService windowManagerService, ISettingsService settingsService)
        {
            _windowManagerService = windowManagerService;
            _settingsService = settingsService;
        }

        [SecureStringValidator]
        public SecureString Password
        {
            get => _password; set => SetProperty(ref _password, value, true);
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Attempt { get; set; }

        public string Prompt { get; set; }

        public string Alert
        {
            get => _alert; set => SetProperty(ref _alert, value);
        }

        public ICommand SubmitCommand => _submitCommand ??= RegisterCommand(OnSubmit, CanSubmit);

        public override void SetModel(object model)
        {
            if (model is not null)
            {
                Attempt = int.Parse(model.ToString());
                UpdateAlert();
            }
        }

        public void SetModel(PasswordViewMode mode, object model)
        {
            switch (mode)
            {
                case PasswordViewMode.Password:
                    Title = "Vault - Password";
                    Prompt = "Password:";
                    Description = "Enter password to PGP Key Store.";
                    break;
                case PasswordViewMode.Passphrase:
                    Title = "Vault - Passphrase";
                    Prompt = "Passphrase:";
                    Description = "Enter passphrase for selected KeyPair.";
                    break;
            }
            SetModel(model);
        }

        private void UpdateAlert()
        {
            var securitySettings = _settingsService.Get<SecuritySettings>(Constants.SETTINGS_SECURITY);

            Alert = $"Incorrect password. Please try again. {securitySettings.SelfEraseOnFailureCount - Attempt + 1} attempts left";
        }

        private bool CanSubmit() => !HasErrors && Password is not null;

        private void OnSubmit() => _windowManagerService.GetWindow(PageKey)?.CloseDialog(true);
    }
}
