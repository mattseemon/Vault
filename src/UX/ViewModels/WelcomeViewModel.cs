using Microsoft.Toolkit.Mvvm.ComponentModel;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;

namespace Seemon.Vault.ViewModels
{
    public class WelcomeViewModel : ObservableObject, INavigationAware
    {
        private readonly IApplicationInfoService _applicationInfoService;

        private string _title;
        public WelcomeViewModel(IApplicationInfoService applicationInfoService) => _applicationInfoService = applicationInfoService;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public void OnNavigateFrom() { }

        public void OnNavigateTo(object parameter)
        {
            Title = _applicationInfoService.GetTitle();
        }
    }
}
