using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Helpers.Extensions;
using Seemon.Vault.Core.Models.Security;
using Seemon.Vault.Core.Models.Settings;
using Seemon.Vault.Helpers.Extensions;
using Seemon.Vault.Helpers.Validators;
using Seemon.Vault.Helpers.ViewModels;
using Seemon.Vault.Models.Views;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        private readonly IWindowManagerService _windowManagerService;
        private readonly IPasswordService _passwordService;
        private readonly ISettingsService _settingsService;

        private SecureString _oldPassword;
        private SecureString _newPassword;
        private SecureString _confirmPassword;

        private ICommand _submitCommand;

        private SecureString _verifyPassword;
        private List<PasswordHash> _exclusionList;
        private readonly int _mininmumUniquePasswordCount;

        public ChangePasswordViewModel(IWindowManagerService windowManagerService, IPasswordService passwordService, ISettingsService settingsService)
        {
            _windowManagerService = windowManagerService;
            _passwordService = passwordService;
            _settingsService = settingsService;
            _mininmumUniquePasswordCount = _settingsService.Get<SecuritySettings>(Constants.SETTINGS_SECURITY).MinimumUniquePasswordCount;
        }

        public ICommand SubmitCommand => _submitCommand ??= RegisterCommand(OnSubmit, CanSubmmit);

        [SecureStringValidator(Description: "Current password")]
        [CustomValidation(typeof(ChangePasswordViewModel), nameof(ValidateOldPassword))]
        public SecureString OldPassword
        {
            get => _oldPassword; set => SetProperty(ref _oldPassword, value, true);
        }

        [SecureStringValidator(Description: "New password")]
        [CustomValidation(typeof(ChangePasswordViewModel), nameof(ValidateExclusionList))]
        public SecureString NewPassword
        {
            get => _newPassword; set => SetProperty(ref _newPassword, value, true);
        }

        [SecureStringValidator(Compare: nameof(NewPassword), Description: "Confirm password", CompareDescription: "New Password")]
        public SecureString ConfirmPassword
        {
            get => _confirmPassword; set => SetProperty(ref _confirmPassword, value, true);
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string OldPasswordLabel { get; set; }

        public string NewPasswordLabel { get; set; }

        public string ConfirmPasswordLabel { get; set; }

        public override void SetModel(object model)
        {
            if (model is not null)
            {
                try
                {
                    _verifyPassword = model.GetPropertyValue<SecureString>("Password");
                    _exclusionList = model.GetPropertyValue<List<PasswordHash>>("ExclusionList");
                }
                catch { }
            }
        }

        public void SetModel(PasswordViewMode mode, object model)
        {
            switch (mode)
            {
                default:
                case PasswordViewMode.Password:
                    Title = "Vault - Password";
                    Description = "Change the password used to encrypt the PGP Key Store. You will be asked this password everytime the application starts.";
                    OldPasswordLabel = "Enter current password:";
                    NewPasswordLabel = "Enter new password:";
                    ConfirmPasswordLabel = "Confirm new password:";
                    break;
                case PasswordViewMode.Passphrase:
                    Title = "Vault - Passphrase";
                    Description = "Change the passhrase used to protect the selected PGP key.";
                    OldPasswordLabel = "Enter current passphrase:";
                    NewPasswordLabel = "Enter new passphrase:";
                    ConfirmPasswordLabel = "Confirm new passphrase:";
                    break;
            }
            SetModel(model);
        }

        public bool CanSubmmit() => !HasErrors && OldPassword is not null && NewPassword is not null && ConfirmPassword is not null;

        public void OnSubmit() => _windowManagerService.GetWindow(PageKey)?.CloseDialog(true);

        private bool VerifyPasswordAgainstExclusionList(SecureString password)
        {
            if (_exclusionList is not null)
            {
                foreach (var hashedPassword in _exclusionList)
                {
                    if (_passwordService.VerifyPassword(password, hashedPassword))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static ValidationResult ValidateOldPassword(SecureString password, ValidationContext context)
        {
            var viewModel = (ChangePasswordViewModel)context.ObjectInstance;
            return viewModel._verifyPassword is null
                ? ValidationResult.Success
                : viewModel._verifyPassword.ToUnsecuredString() == password.ToUnsecuredString()
                ? ValidationResult.Success
                : (new("Current password entered is incorrect"));
        }

        public static ValidationResult ValidateExclusionList(SecureString password, ValidationContext context)
        {
            var viewModel = (ChangePasswordViewModel)context.ObjectInstance;
            return viewModel.VerifyPasswordAgainstExclusionList(password)
                ? ValidationResult.Success
                : (new($"New password cannot be any of the last {viewModel._mininmumUniquePasswordCount} password(s)"));
        }
    }
}
