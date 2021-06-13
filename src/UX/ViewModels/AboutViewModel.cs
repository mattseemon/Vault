using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Helpers.ViewModels;
using System;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class AboutViewModel : ViewModelBase, INavigationAware
    {
        private readonly ApplicationUrls _appUrls;
        private readonly IApplicationInfoService _applicationInfoService;
        private readonly ISystemService _systemService;
        private readonly INavigationService _navigationService;
        private readonly IUpdateService _updateService;


        private ICommand _openInBrowserCommand;
        private ICommand _showLicenseCommand;
        private ICommand _checkUpdateCommand;

        private bool _isCheckingUpdate;
        private string _updateLabel;
        private bool? _updateFound;


        public AboutViewModel(IOptions<ApplicationUrls> appUrls, IApplicationInfoService applicationInforService,
            ISystemService systemService, INavigationService navigationService, IUpdateService updateService)
        {
            _appUrls = appUrls.Value;
            _applicationInfoService = applicationInforService;
            _systemService = systemService;
            _navigationService = navigationService;
            _updateService = updateService;
            IsCheckingUpdate = false;
        }

        public string Title => _applicationInfoService.GetTitle();

        public string Version => $"Version {_applicationInfoService.GetVersion()}";

        public string Author => _applicationInfoService.GetAuthor();

        public string Description => _applicationInfoService.GetDescription();

        public string Copyright => _applicationInfoService.GetCopyright();

        public bool IsPreRelease => _applicationInfoService.GetIsPreRelease();

        public bool IsCheckingUpdate
        {
            get => _isCheckingUpdate; set => SetProperty(ref _isCheckingUpdate, value);
        }

        public string UpdateLabel
        {
            get => _updateLabel; set => SetProperty(ref _updateLabel, value);
        }

        public ICommand OpenInBrowserCommand => _openInBrowserCommand ??= new RelayCommand<string>(OnOpenInBrowser);

        public ICommand ShowLicenseCommand => _showLicenseCommand ??= new RelayCommand(OnShowLicense);

        public ICommand CheckUpdateCommand => _checkUpdateCommand ??= new RelayCommand(OnCheckUpdate, CanCheckUpdate);

        public void OnNavigateTo(object parameter) => SetUpdateLabel();

        public void OnNavigateFrom() { }

        private void OnOpenInBrowser(string parameter)
        {
            var url = _appUrls[parameter]; _systemService.OpenInWebBrowser(url);
        }

        private void OnShowLicense() => _navigationService.NavigateTo(typeof(LicenseViewModel).FullName);

        private bool CanCheckUpdate() => !_updateService.IsBusy;

        private async void OnCheckUpdate()
        {
            IsCheckingUpdate = true;
            _updateFound = await _updateService.CheckForUpdates();
            SetUpdateLabel();
            IsCheckingUpdate = false;
        }

        private void SetUpdateLabel()
        {
            UpdateLabel = !_updateFound.HasValue
                ? $"Update last checked {GetUpdateInterval()}."
                : _updateFound.Value
                ? $"New update available (last checked {GetUpdateInterval()})"
                : $"You have the latest version (last checked {GetUpdateInterval()})";
        }

        private string GetUpdateInterval()
        {
            var difference = DateTime.Now - _updateService.LastChecked.Value;

            if ((DateTime.Now - _updateService.LastChecked.Value).Days > 1)
            {
                return $"on {_updateService.LastChecked:dd MMM}, at {_updateService.LastChecked:h:mm tt}";
            }
            else if (DateTime.Now.Day - _updateService.LastChecked.Value.Day > 0)
            {
                return $"yesterday, at {_updateService.LastChecked:h:mm tt}";
            }
            else if (difference.Hours > 6)
            {
                return $"today, at {_updateService.LastChecked:h:mm tt}";
            }
            else if (difference.Hours > 1)
            {
                return $"{difference.Hours} hours ago";
            }
            else if (difference.Hours > 0)
            {
                return $"an hour ago";
            }
            else if (difference.Minutes > 30)
            {
                return $"less than an hour ago";
            }
            else if (difference.Minutes > 5)
            {
                return $"{difference.Minutes} minutes ago";
            }
            else if (difference.Minutes > 1)
            {
                return $"a few minutes ago";
            }
            else if (difference.Minutes > 0)
            {
                return $"a minute ago";
            }
            else if (difference.Seconds > 10)
            {
                return $"less than a minute ago";
            }
            else if (difference.Seconds > 5)
            {
                return $"a few seconds ago";
            }
            return "just now";
        }
    }
}
