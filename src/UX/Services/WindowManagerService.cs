using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models.Settings;
using Seemon.Vault.Helpers.Extensions;
using System.Windows;

namespace Seemon.Vault.Services
{
    class WindowManagerService : IWindowManagerService
    {
        private readonly ISettingsService _settingsService;

        public WindowManagerService(ISettingsService settingsService) => _settingsService = settingsService;

        public Window MainWindow => App.Current.MainWindow;

        public IWindow GetWindow(string pageKey)
        {
            foreach (Window window in Application.Current.Windows)
            {
                var dataContext = window.GetDataContext();
                if (dataContext?.PageKey == pageKey)
                {
                    return window as IWindow;
                }
            }
            return null;
        }

        public void RestoreWindowSettings()
        {
            MainWindow.Topmost = _settingsService.Get<SystemSettings>(Constants.SETTINGS_SYSTEM).AlwaysOnTop;

            var windowSettings = _settingsService.Get<WindowSettings>(Constants.SETTINGS_WINDOWS);

            if (windowSettings is not null)
            {
                MainWindow.Top = windowSettings.WindowTop;
                MainWindow.Left = windowSettings.WindowLeft;
                MainWindow.Width = windowSettings.WindowWidth;
                MainWindow.Height = windowSettings.WindowHeight;
            }

            ResizeToFit();
            MoveIntoView();
        }

        public void SaveWindowSettings()
        {
            var windowSettings = new WindowSettings
            {
                WindowTop = MainWindow.Top,
                WindowLeft = MainWindow.Left,
                WindowHeight = MainWindow.Height,
                WindowWidth = MainWindow.Width
            };

            _settingsService.Set(Constants.SETTINGS_WINDOWS, windowSettings);
        }

        private void ResizeToFit()
        {
            if (MainWindow.Height > SystemParameters.VirtualScreenHeight)
            {
                MainWindow.Height = SystemParameters.VirtualScreenHeight;
            }

            if (MainWindow.Width > SystemParameters.VirtualScreenWidth)
            {
                MainWindow.Width = SystemParameters.VirtualScreenWidth;
            }
        }

        private void MoveIntoView()
        {
            if (MainWindow.Top + MainWindow.Height / 2 > SystemParameters.VirtualScreenHeight)
            {
                MainWindow.Top = SystemParameters.VirtualScreenHeight - MainWindow.Height;
            }

            if (MainWindow.Left + MainWindow.Width / 2 > SystemParameters.VirtualScreenWidth)
            {
                MainWindow.Left = SystemParameters.VirtualScreenWidth - MainWindow.Width;
            }

            if (MainWindow.Top < 0)
            {
                MainWindow.Top = 0;
            }

            if (MainWindow.Left < 0)
            {
                MainWindow.Left = 0;
            }
        }
    }
}
