using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Models;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class AboutViewModel : ObservableObject, INavigationAware
    {
        private readonly ApplicationConfig _appConfig;
        private readonly IApplicationInfoService _applicationInfoService;
        private readonly ISystemService _systemService;
        private readonly INavigationService _navigationService;

        private ICommand _openInBrowserCommand;
        private ICommand _showLicenseCommand;

        private string _title;
        private string _version;
        private string _author;
        private string _description;
        private string _copyright;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Version
        {
            get { return $"Version {_version}"; }
            set { SetProperty(ref _version, value); }
        }

        public string Author
        {
            get { return _author; }
            set { SetProperty(ref _author, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public string Copyright
        {
            get { return _copyright; }
            set { SetProperty(ref _copyright, value); }
        }

        public ICommand OpenInBrowserCommand => _openInBrowserCommand ??= new RelayCommand<string>(OnOpenInBrowser);
        public ICommand ShowLicenseCommand => _showLicenseCommand ??= new RelayCommand(OnShowLicense);

        public AboutViewModel(IOptions<ApplicationConfig> appConfig, IApplicationInfoService applicationInforService, ISystemService systemService, INavigationService navigationService)
        {
            _appConfig = appConfig.Value;
            _applicationInfoService = applicationInforService;
            _systemService = systemService;
            _navigationService = navigationService;
        }

        public void OnNavigateTo(object parameter)
        {
            Title = _applicationInfoService.GetTitle();
            Version = _applicationInfoService.GetVersion();
            Author = _applicationInfoService.GetAuthor();
            Description = _applicationInfoService.GetDescription();
            Copyright = _applicationInfoService.GetCopyright();
        }

        public void OnNavigateFrom() { }

        private void OnOpenInBrowser(string parameter)
        {
            string url = string.Empty;
            url = parameter switch
            {
                "source" => _appConfig.SourceCodeUrl,
                "license" => _appConfig.LicenseUrl,
                "credits" => _appConfig.CreditsUrl,
                _ => _appConfig.ApplicationUrl
            };
            _systemService.OpenInWebBrowser(url);
        }

        private void OnShowLicense() => _navigationService.NavigateTo(typeof(LicenseViewModel).FullName);
    }
}
