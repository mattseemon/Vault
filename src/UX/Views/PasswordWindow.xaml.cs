using Seemon.Vault.Helpers.Views;
using Seemon.Vault.ViewModels;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : WindowBase
    {
        public PasswordWindow(PasswordViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
