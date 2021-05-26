namespace Seemon.Vault.Core.Contracts.Services
{
    public interface ISystemService
    {
        void OpenInWebBrowser(string url);

        string[] ShowFileDialog(string title, string filters, string initialDirectory, bool multiSelect = false, bool checkFileExists = true);

        string ShowFolderDialog(string description, string initialPath, bool showNewFolderButton = true);

        void SetAutoStartWithWindows(bool enable);
    }
}
