namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IApplicationInfoService
    {
        public string GetTitle();

        public string GetVersion();

        public string GetAuthor();

        public string GetDescription();

        public string GetCopyright();

        public string GetApplicationIdentifier();
    }
}
