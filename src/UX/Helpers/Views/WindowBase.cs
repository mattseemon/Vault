using MahApps.Metro.Controls;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Contracts.Views;
using System.Windows;

namespace Seemon.Vault.Helpers.Views
{
    public class WindowBase : MetroWindow, IWindow
    {
        public IViewModel ViewModel => DataContext as IViewModel;

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
