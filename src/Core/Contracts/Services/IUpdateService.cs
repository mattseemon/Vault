using Seemon.Vault.Core.Models.Updater;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IUpdateService
    {
        DateTime? LastChecked { get; }

        GitHubVersion CurrentVersion { get; }

        GitHubVersion UpgradeVersion { get; }

        GitHubRelease Release { get; }

        bool IsBusy { get; set; }

        Task<bool?> CheckForUpdates(CancellationToken cancellationToken = default);

        Task DownloadAndVerifyUpdate(CancellationToken cancellationToken = default);

        void UpdateAndRestart(string destination);
    }
}
