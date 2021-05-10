namespace Seemon.Vault.Core.Contracts.ViewModels
{
    public interface INavigationAware
    {
        void OnNavigateTo(object parameter);

        void OnNavigateFrom();
    }
}
