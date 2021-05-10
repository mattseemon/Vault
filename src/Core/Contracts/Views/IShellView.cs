using System.Windows.Controls;

namespace Seemon.Vault.Core.Contracts.Views
{
    public interface IShellView
    {
        Frame GetNavigationFrame();

        void ShowWindow();

        void CloseWindow();
    }
}
