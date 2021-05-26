using Seemon.Vault.ViewModels;
using System.Windows.Controls;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for LicensePage.xaml
    /// </summary>
    public partial class LicensePage : Page
    {
        public LicensePage(LicenseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
