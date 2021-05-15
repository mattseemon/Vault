namespace Seemon.Vault.Contracts.Services
{
    public interface ITaskbarIconService
    {
        void Initialize();

        void Show();

        void Hide();

        void Destroy();
    }
}
