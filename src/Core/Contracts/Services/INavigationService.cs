using System;
using System.Windows.Controls;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface INavigationService
    {
        event EventHandler<string> Navigated;

        bool CanGoBack { get; }

        void Initialize(Frame sheelFrame);

        bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false);

        void GoBack();

        void UnsubscribeNavigation();

        void CleanNavigation();
    }
}
