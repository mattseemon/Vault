namespace Seemon.Vault.Core.Contracts.ViewModels
{
    public interface IViewModel
    {
        string PageKey { get; }

        void SetModel(object model);
    }
}
