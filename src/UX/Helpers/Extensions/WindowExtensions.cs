using Seemon.Vault.Core.Contracts.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Seemon.Vault.Helpers.Extensions
{
    public static class WindowExtensions
    {
        public static IViewModel GetDataContext(this Window window)
        {
            if (window.Content is Frame frame)
            {
                return frame.GetDataContext();
            }

            return window.DataContext != null ? window.DataContext as IViewModel : null;
        }
    }
}
