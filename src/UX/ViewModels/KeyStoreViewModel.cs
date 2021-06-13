using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Logging;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Core.Exceptions;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models.KeyStore;
using Seemon.Vault.Core.Models.OpenPGP;
using Seemon.Vault.Core.Models.Settings;
using Seemon.Vault.Helpers.Extensions;
using Seemon.Vault.Helpers.ViewModels;
using Seemon.Vault.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class KeyStoreViewModel : ViewModelBase, INavigationAware
    {
        private readonly ILogger<IViewModel> _logger;
        private readonly IPGPService _pgpService;
        private readonly IKeyStoreService _keyStoreService;
        private readonly IPasswordService _passwordService;
        private readonly INotificationService _notificationService;
        private readonly ISystemService _systemService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;

        private ICommand _generateKeyPairCommand;
        private ICommand _deleteKeyPairCommand;
        private ICommand _viewKeyPairPropertiesCommand;
        private ICommand _changeKeyPairPassphraseCommand;
        private ICommand _defaultKeyPairCommand;
        private ICommand _importKeyPairCommand;
        private ICommand _exportPublicKeyCommand;
        private ICommand _backupSecretKeyCommand;
        private ICommand _changeStorePasswordCommand;
        private ICommand _selectionChangedCommand;

        private ObservableCollection<KeyPairInfo> _keyPairs;
        private KeyPairInfo _selectedKeyPair;

        public KeyStoreViewModel(ILogger<IViewModel> logger, IPGPService pgpService, IKeyStoreService keyStoreService,
            IPasswordService passwordService, INotificationService notificationService, ISystemService systemService,
            ISettingsService settingsService, INavigationService navigationService)
        {
            _logger = logger;
            _pgpService = pgpService;
            _keyStoreService = keyStoreService;
            _keyPairs = new ObservableCollection<KeyPairInfo>();
            _passwordService = passwordService;
            _notificationService = notificationService;
            _systemService = systemService;
            _settingsService = settingsService;
            _navigationService = navigationService;
        }

        public ICommand GenerateKeyPairCommand => _generateKeyPairCommand ??= RegisterCommand(OnGenerateKeyPair);

        public ICommand DeleteKeyPairCommand => _deleteKeyPairCommand ??= RegisterCommand(OnDeleteKeyPair, CanDeleteKeyPair);

        public ICommand ChangeKeyPairPassphraseCommand => _changeKeyPairPassphraseCommand ??= RegisterCommand(OnChangeKeyPairPassphrase, CanChangeKeyPairPassphrase);

        public ICommand DefaultKeyPairCommand => _defaultKeyPairCommand ??= RegisterCommand(OnDefaultKeyPair, CanDefaultKeyPair);

        public ICommand ImportKeyPairCommand => _importKeyPairCommand ??= RegisterCommand(OnImportKeyPair);

        public ICommand ExportPublicKeyCommand => _exportPublicKeyCommand ??= RegisterCommand(OnExportPublicKey, CanExportPublicKey);

        public ICommand BackupSecretKeyCommand => _backupSecretKeyCommand ??= RegisterCommand(OnBackupSecretKey, CanBackupSecretKey);

        public ICommand SelectionChangedCommand => _selectionChangedCommand ??= RegisterCommand(OnSelectionChanged);

        public ICommand ChangeStorePasswordCommand => _changeStorePasswordCommand ??= RegisterCommand(OnChangeStorePassword);

        public ICommand ViewKeyPairPropertiesCommand => _viewKeyPairPropertiesCommand ??= RegisterCommand(OnViewKeyPairProperties, CanViewKeyPairProperties);

        public ObservableCollection<KeyPairInfo> KeyPairs
        {
            get => _keyPairs; set => SetProperty(ref _keyPairs, value);
        }

        public KeyPairInfo SelectedKeyPair
        {
            get => _selectedKeyPair; set => SetProperty(ref _selectedKeyPair, value);
        }

        public IEnumerable<KeyPairInfo> SelectedKeyPairs => KeyPairs.Where(x => x.IsSelected);

        public void OnNavigateFrom() { }

        public void OnNavigateTo(object parameter)
        {
            LoadKeyPairInfoFromStore();
        }

        private async void OnGenerateKeyPair()
        {
            var window = App.GetService<GenerateKeyPairWindow>() as IWindow;
            var response = window.ShowDialog(App.GetService<IShellWindow>());

            if (response.HasValue && response.Value)
            {
                var vm = window.ViewModel as GenerateKeyPairViewModel;
                var keyGenParams = new KeyGenParams((int)vm.KeySize, vm.Password, vm.Name, vm.Email, vm.Comment, vm.Expiry);

                var dialogController = await App.GetService<IShellWindow>().ShowProgressPromptAsync("Key pair generation", "Please wait while the PGP keys are being generated.");
                dialogController.SetIndeterminate();

                var asciiArmored = await _pgpService.GenerateKeyAsync(keyGenParams);

                var keyRing = new EncryptionKeyRing(asciiArmored);

                var keyPair = new KeyPairInfo
                {
                    Id = Guid.NewGuid(),
                    KeyId = keyRing.KeyId,
                    KeySize = keyRing.MasterKeyPair.KeySize,
                    Created = keyRing.MasterKeyPair.Created,
                    Modified = keyRing.MasterKeyPair.Created,
                    Expiry = keyRing.MasterKeyPair.Expiry,
                    HasPrivateKeys = keyRing.MasterKeyPair.HasPrivateKey
                };
                keyPair.SetIdentity(keyRing.MasterKeyPair.User);
                _keyStoreService.InsertKeyPair(keyPair.Id, keyPair, asciiArmored);

                KeyPairs.Add(keyPair);

                await dialogController.CloseAsync();
                _notificationService.ShowMessage(Models.NotificationType.Success, "A new PGP key pair has been generated and stored in the key store.\n\nNote: Remember to backup the secret key in a secure location.", "Key pair generation");
            }
        }

        private bool CanDeleteKeyPair() => SelectedKeyPair is not null && SelectedKeyPairs.Any();

        private async void OnDeleteKeyPair()
        {
            var options = new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No", ColorScheme = MetroDialogColorScheme.Accented };
            var result = await App.GetService<IShellWindow>().ShowMessagePromptAsync("Delete key pair(s)",
                "Are you sure you want to delete the selected key pairs from the Key store?\n\nNote: Any files encrypted with these keys can no longer be decrypted. Please export the keys before deleting.",
                MessageDialogStyle.AffirmativeAndNegative, options);

            if (result == MessageDialogResult.Affirmative)
            {
                var ids = SelectedKeyPairs.Select(x => x.Id).ToList();
                _keyStoreService.DeleteKeyPairs(ids);
                LoadKeyPairInfoFromStore();

                _notificationService.ShowMessage(Models.NotificationType.Success, $"Deleted {ids.Count} key pairs from the key store.", "Delete key pair(s)");
            }
        }

        private bool CanViewKeyPairProperties() => SelectedKeyPair is not null && SelectedKeyPairs.Count() == 1;

        private void OnViewKeyPairProperties()
        {
            _navigationService.NavigateTo(typeof(KeyPairViewModel).FullName, SelectedKeyPair.Id);
        }

        private bool CanChangeKeyPairPassphrase() => SelectedKeyPairs.Count() == 1 && SelectedKeyPair is not null && SelectedKeyPair.HasPrivateKeys;

        private void OnChangeKeyPairPassphrase()
        {
            try
            {
                var response = _passwordService.ChangePassword(SelectedKeyPair.Id.ToString());
                if(response is null)
                {
                    return;
                }

                var armored = _keyStoreService.GetKeyPairAscii(SelectedKeyPair.Id);
                armored = _pgpService.ChangePassphrase(armored, response.OldPassword, response.NewPassword);

                _keyStoreService.UpdateKeyPair(SelectedKeyPair.Id, SelectedKeyPair, armored);
                _notificationService.ShowMessage(Models.NotificationType.Success, "The passphrase has been changed successfully for the selected key pair.", "Change key pair passphrase");
            }
            catch
            {
                _passwordService.ClearPassword(SelectedKeyPair.Id.ToString());
                _notificationService.ShowMessage(Models.NotificationType.Error, "Could not change the passphrase as the current passphrase entered was wrong.", "Change key pair passphrase");
            }
        }

        private bool CanDefaultKeyPair() => SelectedKeyPairs.Count() == 1 && SelectedKeyPair is not null && !SelectedKeyPair.IsDefault;

        private void OnDefaultKeyPair()
        {
            foreach (var keyPair in KeyPairs)
            {
                keyPair.IsDefault = keyPair == SelectedKeyPair;
            }
            _keyStoreService.SetDefaultKeyPair(SelectedKeyPair.Id);
            RaiseCommandsCanExecute();
        }

        private async void OnImportKeyPair()
        {
            var importCount = 0;

            var pgpSettings = _settingsService.Get<PGPSettings>(Constants.SETTINGS_PGP);

            var initialDirectory = !string.IsNullOrEmpty(pgpSettings.LastImportPath) ? pgpSettings.LastImportPath : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var filePaths = _systemService.ShowFileDialog("Select PGP key file", 
                "Key files (*.asc; *.key; *.gpg; *.pgp)|*.asc;*.key;*.gpg;*.pgp|All files (*.*)|*.*", 
                initialDirectory, true, false);

            if(filePaths.Length == 0)
            {
                return;
            }

            foreach(var path in filePaths)
            {
                pgpSettings.LastImportPath = Path.GetDirectoryName(path);
                var update = false;

                KeyPairInfo keyPair = null;
                KeyPairInfo keyCheck = null;

                if (!File.Exists(path))
                {
                    _notificationService.ShowMessage(Models.NotificationType.Warning, $"Could not import file \"{path}\".\n\nSelected file does not exist.", "Import key pair");
                    continue;
                }

                try
                {
                    var asciiArmor = _pgpService.ImportKeys(path);
                    var keyRing = new EncryptionKeyRing(asciiArmor);

                    if (!keyRing.MasterKeyPair.HasPrivateKey)
                    {
                        var result = await App.GetService<IShellWindow>().ShowMessagePromptAsync("Import PGP key pair", "The key file you are trying to import only has the public key. This can be used to encrypt files, but cannot be used to decrypt the same.\n\nDo you want to continue?",
                            MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No" });
                        if(result == MessageDialogResult.Negative)
                        {
                            continue;
                        }    
                    }

                    keyPair = KeyPairs
                        .Where(x => x.KeyId == keyRing.MasterKeyPair.KeyId)
                        .FirstOrDefault();

                    if(keyPair is not null)
                    {
                        var result = await App.GetService<IShellWindow>().ShowMessagePromptAsync("Import PGP key pair", "The key file you are trying to import is already in the Key store. Importing this will overwrite the existing key in the store.\n\nDo you want to continue?",
                            MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No" });
                        if(result == MessageDialogResult.Negative)
                        {
                            continue;
                        }

                        update = true;
                        keyCheck = keyPair.DeepCopy();
                    }
                    else
                    {
                        keyPair = new KeyPairInfo
                        {
                            Id = Guid.NewGuid()
                        };
                    }

                    keyPair.KeyId = keyRing.KeyId;
                    keyPair.SetIdentity(keyRing.MasterKeyPair.User);
                    keyPair.HasPrivateKeys = keyRing.MasterKeyPair.HasPrivateKey;
                    keyPair.KeySize = keyRing.MasterKeyPair.KeySize;
                    keyPair.Created = keyRing.MasterKeyPair.Created;
                    keyPair.Expiry = keyRing.MasterKeyPair.Expiry;
                    keyPair.Modified = keyRing.MasterKeyPair.Created;

                    if (update)
                    {
                        _keyStoreService.UpdateKeyPair(keyPair.Id, keyPair, asciiArmor);
                    }
                    else
                    {
                        _keyStoreService.InsertKeyPair(keyPair.Id, keyPair, asciiArmor);
                    }
                }
                catch (Exception ex) 
                {
                    _notificationService.ShowMessage(Models.NotificationType.Error, "Import PGP key pair failed. Please select a valid PGP key file.", "Import PGP key pair");
                    _logger.LogError("Import PGP key pair failed.", ex);
                }
                importCount++;
                if (update)
                {
                    KeyPairs.Remove(KeyPairs.FirstOrDefault(x => x.Id == keyPair.Id));
                }
                KeyPairs.Add(keyPair);
                _notificationService.ShowMessage(Models.NotificationType.Success, $"Total {importCount} PGP key files were imported.", "Import PGP key pair");
            }
        }

        private bool CanExportPublicKey() => SelectedKeyPair is not null && SelectedKeyPairs.Any();

        private void OnExportPublicKey()
        {
            var pgpSettings = _settingsService.Get<PGPSettings>(Constants.SETTINGS_PGP);
            var lastExportPath = !string.IsNullOrEmpty(pgpSettings.LastExportPath) ? pgpSettings.LastExportPath : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var exportPath = _systemService.ShowFolderDialog("Select path to export PGP public keys to", lastExportPath);

            foreach(var keyPair in SelectedKeyPairs)
            {
                var asciiArmored = _keyStoreService.GetKeyPairAscii(keyPair.Id);
                _pgpService.ExportPublicKeys(asciiArmored, exportPath, pgpSettings.ExportFormat == PGPSettings.FileFormats.AsciiArmor);
            }

            pgpSettings.LastExportPath = exportPath;
            _notificationService.ShowMessage(Models.NotificationType.Success, $"{SelectedKeyPairs.Count()} PGP public key(s) has been exported to the specified location.", "Export PGP public key(s)");
        }

        private bool CanBackupSecretKey() => SelectedKeyPair is not null && SelectedKeyPairs.Count() == 1 && SelectedKeyPair.HasPrivateKeys;

        private void OnBackupSecretKey()
        {
            var pgpSetting = _settingsService.Get<PGPSettings>(Constants.SETTINGS_PGP);
            var lastExportPath = !string.IsNullOrEmpty(pgpSetting.LastExportPath) ? pgpSetting.LastExportPath : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var exportPath = _systemService.ShowFolderDialog("Select path to backup PGP secret keys to", lastExportPath);
            var password = _passwordService.GetPassword(SelectedKeyPair.Id.ToString());
            var asciiArmored = _keyStoreService.GetKeyPairAscii(SelectedKeyPair.Id);

            try
            {
                _pgpService.BackupSecretKeys(asciiArmored, password, exportPath, pgpSetting.ExportFormat == PGPSettings.FileFormats.AsciiArmor);

                pgpSetting.LastExportPath = exportPath;
                _notificationService.ShowMessage(Models.NotificationType.Success, $"The selected PGP secret key has been backed up to the specified location.", "Backup PGP secret key");
            }
            catch(VaultException ex)
            {
                _logger.LogError(ex.Message, ex);
                _passwordService.ClearPassword(SelectedKeyPair.Id.ToString());
                _notificationService.ShowMessage(Models.NotificationType.Error, "Could not backup the selected PGP secret key as the passphrase entered was wrong.", "Backup PGP secret key");
            }
        }

        private void OnSelectionChanged() => RaiseCommandsCanExecute();

        private async void OnChangeStorePassword()
        {
            _logger.LogInformation("Change key store password.");
            var response = await _keyStoreService.ChangePasswordAsync();
            if (response)
            {
                LoadKeyPairInfoFromStore();
                _notificationService.ShowMessage(Models.NotificationType.Success, "Vault key store password has successfully been changed.", "Key store password change");
            }
        }

        private void LoadKeyPairInfoFromStore()
        {
            KeyPairs.Clear();
            foreach (var keyPair in _keyStoreService.GetKeyPairs())
            {
                KeyPairs.Add(keyPair);
            }
        }
    }
}
