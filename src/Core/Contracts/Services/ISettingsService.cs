namespace Seemon.Vault.Core.Contracts.Services
{
    public interface ISettingsService
    {
        public T Get<T>(string key, T @default);

        public T Get<T>(string key);

        public void Set<T>(string key, T value);
    }
}
