using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Helpers;
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

        private string _pageTitle = null;

        private RelayCommand _goBackCommand;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;
        private ICommand _closingCommand;
        private ICommand _showAbout;
        private ICommand _showSettings;

        public RelayCommand GoBackCommand => _goBackCommand ??= new RelayCommand(OnGoBack, CanGoBack);

        public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

        public ICommand UnloadedCommand => _unloadedCommand ??= new RelayCommand(OnUnloaded);

        public ICommand ClosingCommand => _closingCommand ??= new RelayCommand<object>(OnClosing);

        public ICommand ShowAboutCommand => _showAbout ??= new RelayCommand(OnShowAbout);

        public ICommand ShowSettingsCommand => _showSettings ??= new RelayCommand(OnShowSettings);

        public ShellViewModel(INavigationService navigationService, IApplicationInfoService applicationInfoService, 
            ITaskbarIconService taskbarIconService, ISettingsService settingsService)
        {
            _navigationService = navigationService;
            _applicationInfoService = applicationInfoService;
            _taskbarIconService = taskbarIconService;
            _settingsService = settingsService;
        }

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

            if(settings.ShowVaultInNotificationArea && settings.CloseToNotificationArea)
            {
                _taskbarIconService.Hide();
                e.Cancel = true;
            }
        }

        private bool CanGoBack() => _navigationService.CanGoBack;

        private void OnGoBack() => _navigationService.GoBack();

        private void OnShowAbout() => _navigationService.NavigateTo(typeof(AboutViewModel).FullName);

        private void OnShowSettings() => _navigationService.NavigateTo(typeof(SettingsViewModel).FullName);

        private void OnNavigated(object sender, string e)
        {
            GoBackCommand.NotifyCanExecuteChanged();

            Frame frame = (Frame)sender;
            PageTitle = frame.Content is Page page ? page.Title : string.Empty;
        }
    }
}
