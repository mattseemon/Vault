using Seemon.Vault.Helpers.Views;
using Seemon.Vault.ViewModels;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for NewPasswordWindow.xaml
    /// </summary>
    public partial class NewPasswordWindow : WindowBase
    {
        public NewPasswordWindow(NewPasswordViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
