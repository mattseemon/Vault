using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Helpers.Extensions;
using System.Windows;

namespace Seemon.Vault.Services
{
    class WindowManagerService : IWindowManagerService
    {
        public Window MainWindow => App.Current.MainWindow;

        public IWindow GetWindow(string pageKey)
        {
            foreach (Window window in Application.Current.Windows)
            {
                var dataContext = window.GetDataContext();
                if (dataContext?.PageKey == pageKey)
                {
                    return window as IWindow;
                }
            }
            return null;
        }
    }
}
