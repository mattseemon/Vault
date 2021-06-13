using Seemon.Vault.Core.Contracts.ViewModels;

namespace Seemon.Vault.Core.Contracts.Views
{
    public interface IWindow : ICloseable
    {
        IViewModel ViewModel { get; }

        void Show();

        bool? ShowDialog(IWindow owner);
    }
}
