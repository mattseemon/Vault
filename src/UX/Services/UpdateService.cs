using MahApps.Metro.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json.Linq;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Controls.Notifications;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Core.Exceptions;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Core.Models.Settings;
using Seemon.Vault.Core.Models.Updater;
using Seemon.Vault.Helpers.Extensions;
using Seemon.Vault.ViewModels;
using Seemon.Vault.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Seemon.Vault.Services
{
    public class UpdateService : ObservableObject, IUpdateService
    {
        private readonly ILogger<IUpdateService> _logger;
        private readonly ISettingsService _settingsService;
        private readonly IApplicationInfoService _applicationInfoService;
        private readonly IHttpService _httpService;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;

        private GitHubVersion _currentVersion;
        private string _updateUrl;
        private bool _isBusy;
        private Duration _updateTimeout = TimeSpan.FromSeconds(30);
        private GitHubRelease _release;
        private GitHubVersion _upgradeVersion;
        private INotificationMessage _updateNotificationMessage;
        private INotificationMessage _downloadNotificationMessage;
        private CancellationTokenSource _cancellationTokenSource = null;

        public UpdateService(ILogger<IUpdateService> logger, ISettingsService settingsService,
            IApplicationInfoService applicationInfoService, IHttpService httpService, IOptions<ApplicationUrls> appUrls,
            INotificationService notificationService, IFileService fileService)
        {
            _updateUrl = appUrls.Value["update"];
            _logger = logger;
            _settingsService = settingsService;
            _applicationInfoService = applicationInfoService;
            _httpService = httpService;
            _notificationService = notificationService;
            _fileService = fileService;
        }

        public DateTime? LastChecked => _settingsService.Get<UpdateSettings>(Constants.SETTINGS_UDPATES).LastChecked;

        public GitHubVersion CurrentVersion => _currentVersion;

        public GitHubRelease Release => _release;

        public GitHubVersion UpgradeVersion => _upgradeVersion;

        public bool IsBusy
        {
            get => _isBusy; set => SetProperty(ref _isBusy, value);
        }

        public string UpdateMessage { get; private set; }

        public double UpdateProgress { get; private set; }

        public async Task<bool?> CheckForUpdates(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Checking for updates");

                IsBusy = true;

                _currentVersion = new GitHubVersion(_applicationInfoService.GetVersion());

                var response = await _httpService.GetAsync(_updateUrl, cancellationToken);

                _settingsService.Get<UpdateSettings>(Constants.SETTINGS_UDPATES).LastChecked = DateTime.Now;
                var releases = JArray.Parse(response).SelectTokens(_settingsService.Get<UpdateSettings>(Constants.SETTINGS_UDPATES).IncludePreReleases ? "$.[*]" : "$.[?(@.prerelease == false)]");

                _release = null;
                if (!releases.Any())
                {
                    _logger.LogInformation("No published releases for this.");
                    return false;
                }

                var nextVersion = CurrentVersion;
                foreach (var release in releases)
                {
                    var temp = new GitHubRelease(release.ToString());
                    if (temp.Version > nextVersion)
                    {
                        _release = temp;
                        nextVersion = temp.Version;
                    }
                }

                if (_release is null)
                {
                    _logger.LogInformation("You have the latest version");
                    return false;
                }

                _upgradeVersion = _release.Version;

                _logger.LogInformation($"Update found - Version {_release.Version}");

                ShowUpdateNotification();

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Count not connect to the internet.", ex);
                _notificationService.ShowMessage(Models.NotificationType.Error, "Could not check for updates. Check your internet connection and try again later.", "Download failed");
                return false;
            }
        }

        public async Task DownloadAndVerifyUpdate(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Downloading latest update.");

            UpdateMessage = "Downloading updates";

            ShowDownloadNotification();

            var dowloadedPaths = new List<string>();

            var assets = Release.Assets.Where(x => x.Name.Contains("release"));

            foreach (var asset in assets)
            {
                Progress<double> progress = null;
                if (asset.DownloadUrl.EndsWith(".zip"))
                    progress = new Progress<double>(SetUpdateProgress);

                var path = await _httpService.DownloadFileAsync(asset.DownloadUrl, progress, cancellationToken);

                dowloadedPaths.Add(path);
            }

            var hashFilePath = dowloadedPaths.FirstOrDefault(x => x.EndsWith(".json"));
            var updateFilePath = dowloadedPaths.FirstOrDefault(x => x.EndsWith(".zip"));

            bool? hashVerified = null;
            if (!string.IsNullOrEmpty(hashFilePath))
            {
                hashVerified = false;
                SetUpdateMessage("Verifying downloads...");

                var hashes = _fileService.Read<JObject>(hashFilePath);
                hashVerified = _fileService.VerifyFileIntegrity(updateFilePath, IFileService.HashAlgorithms.SHA512, hashes["sha512"].ToString());
            }

            if (hashVerified.HasValue && !hashVerified.Value)
            {
                throw new VaultException("Download file hash does not match with published version.");
            }

            if (File.Exists(updateFilePath))
            {
                SetUpdateMessage("Preparing new update...");
                var applicationPath = _applicationInfoService.GetApplicationExecutablePath();
                var extractionPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                ZipFile.ExtractToDirectory(updateFilePath, extractionPath);

                var files = Directory.GetFiles(extractionPath, "vault.exe", SearchOption.AllDirectories);

                var updatePath = new FileInfo(files[0]).DirectoryName;

                if (Directory.Exists(Path.Combine(applicationPath, "data")))
                {
                    _fileService.CopyDirectory(Path.Join(applicationPath, "data"), Path.Join(Path.GetDirectoryName(files[0]), "data"));
                }

                SetUpdateMessage("Restarting application...");
                var psi = new ProcessStartInfo
                {
                    FileName = files[0],
                    Arguments = $"--UpdatePath=\"{applicationPath}\"",
                    Verb = "RunAs"
                };

                _logger.LogInformation("Restarting application: {0}", new { psi.FileName, psi.Arguments });
                Process.Start(psi);
                Application.Current.Shutdown();
            }
        }

        public void UpdateAndRestart(string destination)
        {
            _logger.LogInformation("Updating application to new version - {Version}", _applicationInfoService.GetVersion());
            var applicationPath = _applicationInfoService.GetApplicationExecutablePath();
            var success = false;

            var startTime = DateTime.Now;
            var backupPath = $"{destination}-backup";

            var attempt = 0;
            _logger.LogInformation("Updating application - {0}", new { Source = applicationPath, Backup = backupPath, Destination = destination });
            while (DateTime.Now - startTime < _updateTimeout && !success)
            {
                _logger.LogInformation("Updating application - {0}", new { Attempt = ++attempt });
                try
                {
                    _logger.LogInformation("Moving previous version in destination to backup path");
                    if (Directory.Exists(destination))
                    {
                        Directory.Move(destination, backupPath);
                    }

                    Directory.CreateDirectory(destination);

                    _logger.LogInformation("Copying udpated version to destination");
                    success = _fileService.CopyDirectory(applicationPath, destination);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error updating application path. Retrying", ex);
                    Thread.Sleep(500);
                }
            }

            if (!success)
            {
                _logger.LogInformation("Restoring previous version in destination");
                if (Directory.Exists(backupPath))
                {
                    Directory.Move(backupPath, destination);
                }
            }
            else
            {
                _logger.LogInformation("Deleting backup of previous version");
                if (Directory.Exists(backupPath))
                {
                    Directory.Delete(backupPath, true);
                }
            }

            _logger.LogInformation("Update Status: {success}", success);

            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(destination, "Vault.exe"),
                Verb = "RunAs",
                Arguments = $"--UpdateStatus={success}"
            };

            _logger.LogInformation("Restarting application: {0}", new { psi.FileName, psi.Arguments });
            Process.Start(psi);
            Application.Current.Shutdown();
        }

        private void SetUpdateMessage(string message)
        {
            _logger.LogInformation(message);
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                UpdateMessage = message;
                if (_downloadNotificationMessage is not null)
                {
                    _downloadNotificationMessage.Message = UpdateMessage;
                    if (_downloadNotificationMessage.OverlayContent is ProgressBar progressBar)
                    {
                        progressBar.IsIndeterminate = true;
                    }
                }
            }, DispatcherPriority.Normal);
        }

        private void SetUpdateProgress(double value)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                UpdateMessage = $"Downloading update... {value}%";
                UpdateProgress = value;

                if (_downloadNotificationMessage is not null)
                {
                    _downloadNotificationMessage.Message = UpdateMessage;
                    if (_downloadNotificationMessage.OverlayContent is ProgressBar progressBar)
                    {
                        progressBar.IsIndeterminate = false;
                        progressBar.Value = UpdateProgress;
                    }
                }
            }, DispatcherPriority.Normal);
        }

        private void ShowUpdateNotification()
        {
            IsBusy = true;

            Application.Current.Dispatcher.Invoke(() =>
            {
                DismissNotifications();

                _cancellationTokenSource = new CancellationTokenSource();

                _updateNotificationMessage = _notificationService.Default()
                    .HasBadge("Update")
                    .Accent(Constants.NOTIFICATION_COLOR_UPDATE)
                    .HasHeader("Update Available")
                    .HasMessage($"A new version of {_applicationInfoService.GetTitle()} is availble for download.\n\nCurrent Version:\t {_applicationInfoService.GetVersion()}\nNew Version:\t {Release.Version}")
                    .Dismiss().WithButton("Download & Update", async button =>
                    {
                        try
                        {
                            await DownloadAndVerifyUpdate(_cancellationTokenSource.Token);
                        }
                        catch (TaskCanceledException)
                        {
                            _notificationService.ShowMessage(Models.NotificationType.Warning, "Application update has been cancelled.");
                        }
                        catch (TimeoutException ex)
                        {
                            _logger.LogError("Could not download the update. Request timed out.", ex);
                            _notificationService.ShowMessage(Models.NotificationType.Error, "Could not download the update.", "Download failed");
                        }
                        catch (HttpRequestException ex)
                        {
                            _logger.LogError("Count not connect to the internet.", ex);
                            _notificationService.ShowMessage(Models.NotificationType.Error, "Could not connect to the server to download the update. Check your internet connection and try again later.", "Download failed");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Unknow error", ex);
                            _notificationService.ShowMessage(Models.NotificationType.Error, "Could not download the update due to unknow error. Try again later.", "Download failed");
                        }
                    })
                    .WithButton("Release notes", button =>
                    {
                        IWindow window = App.GetService<ReleaseNotesWindow>();

                        var vm = window.ViewModel as ReleaseNotesViewModel;
                        vm.SetModel(Release.Contents);

                        var response = window.ShowDialog(App.GetService<IShellWindow>());
                    })
                    .Dismiss().WithButton("Later", button => { IsBusy = false; })
                    .Queue();
            });
        }

        private void ShowDownloadNotification()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                DismissNotifications();

                var progressBar = new MetroProgressBar
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 6,
                    Minimum = 0,
                    Maximum = 100,
                    BorderThickness = new Thickness(0),
                    Foreground = Constants.NOTIFICATION_COLOR_UPDATE,
                    Background = Brushes.Transparent,
                    IsIndeterminate = true,
                    IsHitTestVisible = false
                };

                _downloadNotificationMessage = _notificationService.Default()
                    .HasBadge("Update")
                    .Accent(Constants.NOTIFICATION_COLOR_UPDATE)
                    .HasMessage(UpdateMessage)
                    .WithOverlay(progressBar)
                    .WithButton("Cancel", button => { _cancellationTokenSource.Cancel(); })
                    .Queue();

            }, DispatcherPriority.Normal);
        }

        private void DismissNotifications()
        {
            _updateNotificationMessage?.Dismiss();
            _downloadNotificationMessage?.Dismiss();
        }
    }
}
