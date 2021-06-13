using Seemon.Vault.ViewModels;
using System.Windows.Controls;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for KeyStorePage.xaml
    /// </summary>
    public partial class KeyStorePage : Page
    {
        public KeyStorePage(KeyStoreViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
