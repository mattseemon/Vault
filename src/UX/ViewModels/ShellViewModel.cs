using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Controls.Notifications;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models.Settings;
using Seemon.Vault.Helpers.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApplicationInfoService _applicationInfoService;
        private readonly ITaskbarIconService _taskbarIconService;
        private readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;
        private readonly IWindowManagerService _windowManagerService;

        private string _pageTitle = null;
        private ICommand _goBackCommand;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;
        private ICommand _closingCommand;
        private ICommand _showAboutCommand;
        private ICommand _showSettingsCommand;
        private ICommand _showKeyStoreCommand;

        public ShellViewModel(INavigationService navigationService, IApplicationInfoService applicationInfoService,
            ITaskbarIconService taskbarIconService, ISettingsService settingsService, INotificationService notificationService,
            IWindowManagerService windowManagerService)
        {
            _navigationService = navigationService;
            _applicationInfoService = applicationInfoService;
            _taskbarIconService = taskbarIconService;
            _settingsService = settingsService;
            _notificationService = notificationService;
            _windowManagerService = windowManagerService;
        }

        public ICommand GoBackCommand => _goBackCommand ??= RegisterCommand(OnGoBack, CanGoBack);

        public ICommand LoadedCommand => _loadedCommand ??= RegisterCommand(OnLoaded);

        public ICommand UnloadedCommand => _unloadedCommand ??= RegisterCommand(OnUnloaded);

        public ICommand ClosingCommand => _closingCommand ??= RegisterCommand<object>(OnClosing);

        public ICommand ShowAboutCommand => _showAboutCommand ??= RegisterCommand(OnShowAbout);

        public ICommand ShowSettingsCommand => _showSettingsCommand ??= RegisterCommand(OnShowSettings);

        public ICommand ShowKeyStoreCommand => _showKeyStoreCommand ??= RegisterCommand(OnShowKeyStore);

        public INotificationMessageManager Manager => _notificationService.Manager;

        public string PageTitle
        {
            get => string.IsNullOrEmpty(_pageTitle) ? _applicationInfoService.GetTitle() : $"{_applicationInfoService.GetTitle()} - {_pageTitle}";
            set => SetProperty(ref _pageTitle, value);
        }

        private void OnLoaded() => _navigationService.Navigated += OnNavigated;

        private void OnUnloaded() => _navigationService.Navigated -= OnNavigated;

        private void OnClosing(object parameter)
        {
            var e = (CancelEventArgs)parameter;
            var settings = _settingsService.Get<SystemSettings>(Constants.SETTINGS_SYSTEM);
            _windowManagerService.SaveWindowSettings();

            if (settings.ShowVaultInNotificationArea && settings.CloseToNotificationArea)
            {
                _taskbarIconService.Hide();
                e.Cancel = true;
            }
        }

        private bool CanGoBack() => _navigationService.CanGoBack;

        private void OnGoBack() => _navigationService.GoBack();

        private void OnShowAbout() => _navigationService.NavigateTo(typeof(AboutViewModel).FullName);

        private void OnShowSettings() => _navigationService.NavigateTo(typeof(SettingsViewModel).FullName);

        private void OnShowKeyStore() => _navigationService.NavigateTo(typeof(KeyStoreViewModel).FullName);

        private void OnNavigated(object sender, string e)
        {
            (GoBackCommand as RelayCommand).NotifyCanExecuteChanged();

            Frame frame = (Frame)sender;
            PageTitle = frame.Content is Page page ? page.Title : string.Empty;
        }
    }
}
