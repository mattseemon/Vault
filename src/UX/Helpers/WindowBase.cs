using MahApps.Metro.Controls;
using Seemon.Vault.Core.Contracts.Views;
using System.Windows;

namespace Seemon.Vault.Helpers
{
    public class WindowBase : MetroWindow, IWindow
    {
        public object ViewModel => DataContext;

        public void CloseDialog(bool? response = null)
        {
            DialogResult = response;
            Close();
        }

        public bool? ShowDialog(IWindow owner)
        {
            Owner = (Window)owner;
            return ShowDialog();
        }
    }
}
