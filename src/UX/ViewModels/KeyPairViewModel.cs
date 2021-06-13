using Microsoft.Extensions.Logging;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Models;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Exceptions;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models.KeyStore;
using Seemon.Vault.Core.Models.OpenPGP;
using Seemon.Vault.Core.Models.Settings;
using Seemon.Vault.Helpers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class KeyPairViewModel : ViewModelBase, INavigationAware
    {
        private readonly ILogger<IViewModel> _logger;
        private readonly INavigationService _navigationService;
        private readonly IKeyStoreService _keyStoreService;
        private readonly IPasswordService _passwordService;
        private readonly IPGPService _pgpService;
        private readonly INotificationService _notificationService;
        private readonly ISystemService _systemService;
        private readonly ISettingsService _settingsService;

        private SecureString _asciiArmored;
        private IEncryptionKeyRing _keyRing;
        private KeyPairInfo _selectedKeyPair;

        private ICommand _goBackCommand;
        private ICommand _copyCommand;
        private ICommand _changeKeyPairPassphraseCommand;
        private ICommand _exportPublicKeyCommand;
        private ICommand _backupSecretKeyCommand;

        public KeyPairViewModel(ILogger<IViewModel> logger, INavigationService navigationService,
            IKeyStoreService keyStoreService, IPasswordService passwordService, IPGPService pgpService,
            INotificationService notifcationService, ISystemService systemService, ISettingsService settingsService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _keyStoreService = keyStoreService;
            _passwordService = passwordService;
            _pgpService = pgpService;
            _notificationService = notifcationService;
            _systemService = systemService;
            _settingsService = settingsService;
        }

        public IEncryptionKeyRing KeyRing
        {
            get => _keyRing; set => SetProperty(ref _keyRing, value);
        }

        public IList<EncryptionKeyPairUser> Users { get; private set; }

        public KeyPairInfo SelectedKeyPair => _selectedKeyPair;

        public ICommand GoBackCommand => _goBackCommand ??= RegisterCommand(OnGoBack, CanGoBack);

        public ICommand CopyCommand => _copyCommand ??= RegisterCommand<string>(OnCopy);

        public ICommand ChangeKeyPairPassphraseCommand => _changeKeyPairPassphraseCommand ??= RegisterCommand(OnChangeKeyPairPassphrase, CanChangeKeyPairPassphrase);

        public ICommand ExportPublicKeyCommand => _exportPublicKeyCommand ??= RegisterCommand(OnExportPublicKey);

        public ICommand BackupSecretKeyCommand => _backupSecretKeyCommand ??= RegisterCommand(OnBackupSecretKey, CanBackupSecretKey);

        private bool CanGoBack() => _navigationService.CanGoBack;

        private void OnGoBack() => _navigationService.GoBack();

        private void OnCopy(string parameter) => Clipboard.SetText(parameter);

        private bool CanChangeKeyPairPassphrase() => SelectedKeyPair.HasPrivateKeys;

        private void OnChangeKeyPairPassphrase()
        {
            try
            {
                var response = _passwordService.ChangePassword(SelectedKeyPair.Id.ToString());
                if (response is null)
                {
                    return;
                }
                _asciiArmored = _pgpService.ChangePassphrase(_asciiArmored, response.OldPassword, response.NewPassword);
                _keyStoreService.UpdateKeyPair(SelectedKeyPair.Id, SelectedKeyPair, _asciiArmored);

                KeyRing = new EncryptionKeyRing(_asciiArmored);
                _notificationService.ShowMessage(Models.NotificationType.Success, "The passphrase has been changed successfully for the selected key pair.", "Change key pair passphrase");
            }
            catch
            {
                _passwordService.ClearPassword(SelectedKeyPair.Id.ToString());
                _notificationService.ShowMessage(Models.NotificationType.Error, "Could not change the passphrase as the current passphrase entered was wrong.", "Change key pair passphrase");
            }
        }

        private void OnExportPublicKey()
        {
            var pgpSettings = _settingsService.Get<PGPSettings>(Constants.SETTINGS_PGP);
            var lastExportPath = !string.IsNullOrEmpty(pgpSettings.LastExportPath) ? pgpSettings.LastExportPath : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var exportPath = _systemService.ShowFolderDialog("Select path to export PGP public keys to", lastExportPath);

            _pgpService.ExportPublicKeys(_asciiArmored, exportPath, pgpSettings.ExportFormat == PGPSettings.FileFormats.AsciiArmor);

            pgpSettings.LastExportPath = exportPath;
            _notificationService.ShowMessage(Models.NotificationType.Success, $"PGP public keys has been exported to the specified location.", "Export PGP public keys");
        }

        private bool CanBackupSecretKey() => SelectedKeyPair.HasPrivateKeys;

        private void OnBackupSecretKey()
        {
            var pgpSetting = _settingsService.Get<PGPSettings>(Constants.SETTINGS_PGP);
            var lastExportPath = !string.IsNullOrEmpty(pgpSetting.LastExportPath) ? pgpSetting.LastExportPath : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var exportPath = _systemService.ShowFolderDialog("Select path to backup PGP secret keys to", lastExportPath);
            var password = _passwordService.GetPassword(SelectedKeyPair.Id.ToString());

            try
            {
                _pgpService.BackupSecretKeys(_asciiArmored, password, exportPath, pgpSetting.ExportFormat == PGPSettings.FileFormats.AsciiArmor);

                pgpSetting.LastExportPath = exportPath;
                _notificationService.ShowMessage(Models.NotificationType.Success, $"The selected PGP secret key has been backed up to the specified location.", "Backup PGP secret key");
            }
            catch (VaultException ex)
            {
                _logger.LogError(ex.Message, ex);
                _passwordService.ClearPassword(SelectedKeyPair.Id.ToString());
                _notificationService.ShowMessage(Models.NotificationType.Error, "Could not backup the selected PGP secret key as the passphrase entered was wrong.", "Backup PGP secret key");
            }
        }

        public void OnNavigateTo(object parameter)
        {
            if (parameter is not null)
            {
                var id = (Guid)parameter;
                _selectedKeyPair = _keyStoreService.GetKeyPair(id);
                _asciiArmored = _keyStoreService.GetKeyPairAscii(id);
                KeyRing = new EncryptionKeyRing(_asciiArmored);

                Users = new List<EncryptionKeyPairUser>();
                Users = (from x in KeyRing.MasterKeyPair.Users select new EncryptionKeyPairUser(x)).ToList();
            }
        }

        public void OnNavigateFrom() { }
    }
}
