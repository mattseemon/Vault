using Seemon.Vault.Helpers.Views;
using Seemon.Vault.ViewModels;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for GenerateEncryptionKeyPairView.xaml
    /// </summary>
    public partial class GenerateKeyPairWindow : WindowBase
    {
        public GenerateKeyPairWindow(GenerateKeyPairViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
