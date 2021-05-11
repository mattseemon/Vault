using System.Windows.Controls;

namespace Seemon.Vault.Core.Contracts.Views
{
    public interface IShellWindow
    {
        Frame GetNavigationFrame();

        void ShowWindow();
        
        void CloseWindow();
    }
}
