using Seemon.Vault.Core.Contracts.Views;
using System.Windows;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IWindowManagerService
    {
        Window MainWindow { get; }

        IWindow GetWindow(string pageKey);
    }
}
