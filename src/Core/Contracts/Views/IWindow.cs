namespace Seemon.Vault.Core.Contracts.Views
{
    public interface IWindow : ICloseable
    {
        object ViewModel { get; }

        void Show();

        bool? ShowDialog(IWindow owner);
    }
}
