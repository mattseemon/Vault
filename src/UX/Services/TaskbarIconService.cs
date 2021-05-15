﻿using Hardcodet.Wpf.TaskbarNotification;
using Seemon.Vault.Contracts.Services;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Helpers;
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

        private WindowState _storedWindowState = WindowState.Normal;

        public TaskbarIconService(IWindowManagerService windowManagerService, ISettingsService settingsService)
        {
            _windowManagerService = windowManagerService;
            _settingsService = settingsService;
        }

        private TaskbarIcon _taskbarIcon;
        public void Destroy()
        {
            if (_taskbarIcon != null)
            {
                if (_windowManagerService.MainWindow != null)
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
                if (_windowManagerService.MainWindow == null)
                {
                    return;
                }

                if (_taskbarIcon == null)
                {
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
            if (_windowManagerService.MainWindow != null)
            {
                _windowManagerService.MainWindow.Show();
                _windowManagerService.MainWindow.WindowState = _storedWindowState;
                _windowManagerService.MainWindow.Activate();
            }
        }

        public void Hide()
        {
            if (_windowManagerService.MainWindow != null)
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