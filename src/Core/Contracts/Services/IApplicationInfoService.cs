namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IApplicationInfoService
    {
        public string GetTitle();

        public string GetVersion();

        public string GetAuthor();

        public string GetDescription();

        public string GetCopyright();

        public bool GetIsPreRelease();

        public string GetApplicationIdentifier();

        public string GetApplicationExecutablePath();

        public string GetDataPath();

        public string GetLogPath();
    }
}
