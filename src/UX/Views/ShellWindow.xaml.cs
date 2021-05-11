using MahApps.Metro.Controls;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.ViewModels;
using System.Windows.Controls;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellWindow : MetroWindow, IShellWindow
    {
        public ShellWindow(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public Frame GetNavigationFrame() => shellFrame;

        public void ShowWindow() => Show();

        public void CloseWindow() => Close();
    }
}
