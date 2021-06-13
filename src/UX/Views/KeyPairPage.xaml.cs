using Seemon.Vault.ViewModels;
using System.Windows.Controls;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for KeyPairPage.xaml
    /// </summary>
    public partial class KeyPairPage : Page
    {
        public KeyPairPage(KeyPairViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
