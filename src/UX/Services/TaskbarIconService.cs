using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Logging;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models.Settings;
using Seemon.Vault.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Seemon.Vault.Services
{
    class TaskbarIconService : ITaskbarIconService
    {
        private readonly IWindowManagerService _windowManagerService;
        private readonly ISettingsService _settingsService;
        private readonly ILogger<ITaskbarIconService> _logger;

        private WindowState _storedWindowState = WindowState.Normal;
        private TaskbarIcon _taskbarIcon;

        public TaskbarIconService(IWindowManagerService windowManagerService, ISettingsService settingsService, ILogger<ITaskbarIconService> logger)
        {
            _windowManagerService = windowManagerService;
            _settingsService = settingsService;
            _logger = logger;
        }

        public void Destroy()
        {
            if (_taskbarIcon is not null)
            {
                _logger.LogInformation("Destroying TaskbarIcon");
                if (_windowManagerService.MainWindow is not null)
                {
                    _windowManagerService.MainWindow.StateChanged -= OnWindowStateChanged;
                }
                _taskbarIcon.Dispose();
                _taskbarIcon = null;
            }
        }

        public void Initialize()
        {
            var settings = _settingsService.Get<SystemSettings>(Constants.SETTINGS_SYSTEM);

            if (settings.ShowVaultInNotificationArea)
            {
                if (_windowManagerService.MainWindow is null)
                {
                    return;
                }

                if (_taskbarIcon is null)
                {
                    _logger.LogInformation("Initializing TaskbarIcon");
                    _taskbarIcon = new TaskbarIcon
                    {
                        DataContext = App.GetService<TaskbarIconViewModel>(),
                        IconSource = _windowManagerService.MainWindow.Icon,
                        TrayToolTip = (UIElement)App.Current.FindResource("TBITooltip"),
                        ContextMenu = (ContextMenu)App.Current.FindResource("TBIContextMenu")
                    };
                    _taskbarIcon.TrayMouseDoubleClick += OnTrayMouseDoubleClick;
                    _windowManagerService.MainWindow.StateChanged += OnWindowStateChanged;
                }
            }
        }

        public void Show()
        {
            if (_windowManagerService.MainWindow is not null)
            {
                _windowManagerService.MainWindow.Show();
                _windowManagerService.MainWindow.WindowState = _storedWindowState;
                _windowManagerService.MainWindow.Activate();
            }
        }

        public void Hide()
        {
            if (_windowManagerService.MainWindow is not null)
            {
                _windowManagerService.MainWindow.Hide();
            }
        }

        private void OnTrayMouseDoubleClick(object sender, RoutedEventArgs e) => Show();

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            var settings = _settingsService.Get<SystemSettings>(Constants.SETTINGS_SYSTEM);

            if (_windowManagerService.MainWindow.WindowState == WindowState.Minimized)
            {
                if (settings.ShowVaultInNotificationArea && settings.MinimizeToNotificationArea)
                {
                    Hide();
                }
            }
            else
            {
                _storedWindowState = _windowManagerService.MainWindow.WindowState;
            }
        }
    }
}
