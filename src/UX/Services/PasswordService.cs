using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Helpers.Services;
using Seemon.Vault.Core.Models.Security;
using Seemon.Vault.Models.Views;
using Seemon.Vault.ViewModels;
using Seemon.Vault.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Windows;

namespace Seemon.Vault.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IPasswordCacheService _passwordCacheService;

        public PasswordService(IPasswordCacheService passwordCacheService) => _passwordCacheService = passwordCacheService;

        public ChangePasswordResponse ChangePassword(string key, List<PasswordHash> exclusionList = null)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var oldPassword = _passwordCacheService.Get(key);
                var window = App.GetService<ChangePasswordWindow>() as IWindow;
                var viewModel = window.ViewModel as ChangePasswordViewModel;
                viewModel.SetModel(key == Constants.PASSWORD_CACHE_KEY_STORE ? PasswordViewMode.Password : PasswordViewMode.Passphrase, new { Password = oldPassword, ExclusionList = exclusionList });

                var response = window.ShowDialog(App.GetService<IShellWindow>());
                var expires = key != Constants.PASSWORD_CACHE_KEY_STORE;

                if (response.HasValue && response.Value)
                {
                    _passwordCacheService.Set(key, viewModel.NewPassword, expires);
                    return new ChangePasswordResponse { OldPassword = viewModel.OldPassword, NewPassword = viewModel.NewPassword };
                }
                return null;
            });
        }

        public SecureString CreatePassword(string key)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                var window = App.GetService<NewPasswordWindow>() as IWindow;
                var response = window.ShowDialog(App.GetService<IShellWindow>());

                var viewModel = window.ViewModel as NewPasswordViewModel;
                var expires = key != Constants.PASSWORD_CACHE_KEY_STORE;

                if (response.HasValue && response.Value)
                {
                    _passwordCacheService.Set(key, viewModel.Password, expires);
                    return viewModel.Password;
                }

                return null;
            });
        }

        public SecureString GetPassword(string key, int attempt = 0, bool forcePrompt = false)
        {
            SecureString password = null;
            if (!forcePrompt)
            {
                password = _passwordCacheService.Get(key);
            }
            return password ?? Application.Current.Dispatcher.Invoke(() =>
                 {
                     var window = App.GetService<PasswordWindow>() as IWindow;
                     var viewModel = window.ViewModel as PasswordViewModel;

                     viewModel.SetModel(key == Constants.PASSWORD_CACHE_KEY_STORE ? PasswordViewMode.Password : PasswordViewMode.Passphrase, attempt);

                     var response = window.ShowDialog(App.GetService<IShellWindow>());
                     var expires = key != Constants.PASSWORD_CACHE_KEY_STORE;
                     _passwordCacheService.Set(key, viewModel.Password);

                     return response.HasValue && response.Value ? viewModel.Password : null;
                 });
        }

        public PasswordHash HashPassword(SecureString password)
        {
            var salt = EncryptionHelper.GenerateRandomBytes(16);
            var hash = EncryptionHelper.GenerateKey(password, salt, 32);

            return new PasswordHash
            {
                Salt = Convert.ToBase64String(salt),
                Hash = Convert.ToBase64String(hash)
            };
        }

        public void ClearPassword(string key) => _passwordCacheService.Clear(key);

        public bool VerifyPassword(SecureString password, PasswordHash verificationHash)
        {
            var salt = Convert.FromBase64String(verificationHash.Salt);
            var hash = EncryptionHelper.GenerateKey(password, salt, 32);

            return hash.SequenceEqual(Convert.FromBase64String(verificationHash.Hash));
        }
    }
}
