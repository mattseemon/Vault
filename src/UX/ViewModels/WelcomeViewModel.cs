using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Helpers.ViewModels;

namespace Seemon.Vault.ViewModels
{
    public class WelcomeViewModel : ViewModelBase, INavigationAware
    {
        private readonly IApplicationInfoService _applicationInfoService;

        private string _title;

        public WelcomeViewModel(IApplicationInfoService applicationInfoService) => _applicationInfoService = applicationInfoService;

        public string Title
        {
            get => _title; set => SetProperty(ref _title, value);
        }

        public void OnNavigateFrom() { }

        public void OnNavigateTo(object parameter) => Title = _applicationInfoService.GetTitle();
    }
}
