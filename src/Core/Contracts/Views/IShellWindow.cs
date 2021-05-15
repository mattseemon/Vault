using System.Windows.Controls;

namespace Seemon.Vault.Core.Contracts.Views
{
    public interface IShellWindow : IWindow
    {
        Frame GetNavigationFrame();
    }
}
