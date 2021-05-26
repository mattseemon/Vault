using Seemon.Vault.Core.Contracts.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Seemon.Vault.Helpers.Extensions
{
    public static class WindowExtensions
    {
        public static IViewModel GetDataContext(this Window window)
        {
            return window.FindName("ShellFrame") is Frame frame
                ? frame.GetDataContext()
                : window.DataContext != null ? window.DataContext as IViewModel : null;
        }
    }
}
