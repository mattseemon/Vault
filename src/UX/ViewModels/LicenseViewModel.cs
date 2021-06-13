using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Helpers.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Seemon.Vault.ViewModels
{
    public class LicenseViewModel : ViewModelBase, INavigationAware
    {
        private readonly ISystemService _systemService;

        private ApplicationUrls _appUrls;
        private List<string> _lines;
        private ICommand _openInBrowserCommand;

        public LicenseViewModel(IOptions<ApplicationUrls> appUrls, ISystemService systemService)
        {
            _appUrls = appUrls.Value;
            _systemService = systemService;
        }

        public List<string> Lines
        {
            get => _lines; set => SetProperty(ref _lines, value);
        }

        public ICommand OpenInBrowserCommand => _openInBrowserCommand ??= new RelayCommand<string>(OnOpenInBrowser);

        private void OnOpenInBrowser(string parameter) => _systemService.OpenInWebBrowser(_appUrls[parameter]);

        public void OnNavigateTo(object parameter) => Lines = File.ReadLines(@".\LICENSE").ToList();

        public void OnNavigateFrom() { }
    }
}
