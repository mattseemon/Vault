using Seemon.Vault.Helpers.Views;
using Seemon.Vault.ViewModels;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : WindowBase
    {
        public ProfileWindow(ProfileViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
