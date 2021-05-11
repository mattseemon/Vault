using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class LicenseViewModel : ObservableObject, INavigationAware
    {
        private readonly ISystemService _systemService;

        private ApplicationConfig _appConfig;
        private List<string> _lines;
        private ICommand _openInBrowserCommand;

        public LicenseViewModel(IOptions<ApplicationConfig> appConfig, ISystemService systemService)
        {
            _appConfig = appConfig.Value;
            _systemService = systemService;
        }

        public List<string> Lines
        {
            get => _lines;
            set => SetProperty(ref _lines, value);
        }

        public ICommand OpenInBrowserCommand => _openInBrowserCommand ??= new RelayCommand<string>(OnOpenInBrowser);

        private void OnOpenInBrowser(string parameter)
        {
            string url = string.Empty;
            url = parameter switch
            {
                "3rdParty" => _appConfig.ThirdPartyUrl,
                _ => _appConfig.ApplicationUrl
            };
            _systemService.OpenInWebBrowser(url);
        }

        public void OnNavigateTo(object parameter) => Lines = File.ReadLines(@".\LICENSE").ToList();

        public void OnNavigateFrom() { }
    }
}
