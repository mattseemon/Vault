namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IDataService
    {
        void PersistData();

        void RestoreData();
    }
}
