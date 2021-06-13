using Microsoft.Win32;
using Seemon.Vault.Core.Contracts.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Seemon.Vault.Core.Services
{
    public class SystemService : ISystemService
    {
        private readonly IApplicationInfoService _applicationInfoService;
        private const string _autoStartKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public SystemService(IApplicationInfoService applicationInfoService) => _applicationInfoService = applicationInfoService;

        public void OpenInWebBrowser(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }

        public void SetAutoStartWithWindows(bool enable)
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(_autoStartKey, true);
            if (key is null)
            {
                throw new Exception("Could not set application to auto start with windows.");
            }

            var identifier = _applicationInfoService.GetApplicationIdentifier();
            var executablePath = Process.GetCurrentProcess().MainModule.FileName;

            if (enable)
            {
                key.SetValue(identifier, $"\"{executablePath}\"");
            }
            else if (!enable && key.GetValue(identifier, null) is not null)
            {
                key.DeleteValue(identifier, false);
            }
        }

        public string[] ShowFileDialog(string title = "Select file(s)", string filters = "All Files (*.*)|*.*", string initialDirectory = "", bool multiSelect = false, bool checkFileExists = true)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                Title = title,
                Filter = filters,
                CheckFileExists = checkFileExists,
                Multiselect = multiSelect,
                InitialDirectory = initialDirectory
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames : Array.Empty<string>();
        }

        public string ShowFolderDialog(string description, string initialPath, bool showNewFolderButton)
        {
            if (!string.IsNullOrEmpty(initialPath) && !initialPath.EndsWith(Path.DirectorySeparatorChar))
            {
                initialPath += Path.DirectorySeparatorChar;
            }

            using var dialog = new FolderBrowserDialog
            {
                Description = description,
                UseDescriptionForTitle = true,
                AutoUpgradeEnabled = true,
                SelectedPath = initialPath,
                ShowNewFolderButton = showNewFolderButton,
                RootFolder = Environment.SpecialFolder.Desktop
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    }
}
