using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Core.Contracts.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        private readonly IApplicationInfoService _applicationInfoService;

        private string _pageTitle = null;

        private RelayCommand _goBackCommand;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;
        private ICommand _showAbout;
        private ICommand _showSettings;

        public RelayCommand GoBackCommand => _goBackCommand ??= new RelayCommand(OnGoBack, CanGoBack);
        public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);
        public ICommand UnloadedCommand => _unloadedCommand ??= new RelayCommand(OnUnloaded);
        public ICommand ShowAboutCommand => _showAbout ??= new RelayCommand(OnShowAbout);
        public ICommand ShowSettingsCommand => _showSettings ??= new RelayCommand(OnShowSettings);

        public string PageTitle
        {
            get
            {
                if (string.IsNullOrEmpty(_pageTitle))
                {
                    return _applicationInfoService.GetTitle();
                }
                return $"{_applicationInfoService.GetTitle()} - {_pageTitle}";
            }
            set => SetProperty(ref _pageTitle, value);
        }

        public ShellViewModel(INavigationService navigationService, IApplicationInfoService applicationInfoService)
        {
            _navigationService = navigationService;
            _applicationInfoService = applicationInfoService;
        }

        private void OnLoaded() => _navigationService.Navigated += OnNavigated;

        private void OnUnloaded() => _navigationService.Navigated -= OnNavigated;

        private bool CanGoBack() => _navigationService.CanGoBack;

        private void OnGoBack() => _navigationService.GoBack();

        private void OnShowAbout() => _navigationService.NavigateTo(typeof(AboutViewModel).FullName);

        private void OnShowSettings() => _navigationService.NavigateTo(typeof(SettingsViewModel).FullName);

        private void OnNavigated(object sender, string e)
        {
            GoBackCommand.NotifyCanExecuteChanged();

            Frame frame = (Frame)sender;
            if (frame.Content is Page)
            {
                PageTitle = ((Page)frame.Content).Title;
            }
            else
            {
                PageTitle = string.Empty;
            }
        }
    }
}
