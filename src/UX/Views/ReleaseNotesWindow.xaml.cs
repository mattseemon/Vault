using Seemon.Vault.Helpers;
using Seemon.Vault.ViewModels;

namespace Seemon.Vault.Views
{
    /// <summary>
    /// Interaction logic for ReleaseNotesWindow.xaml
    /// </summary>
    public partial class ReleaseNotesWindow : WindowBase
    {
        public ReleaseNotesWindow(ReleaseNotesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
