using ControlzEx.Theming;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Seemon.Vault.ViewModels
{
    public class SettingsViewModel : ObservableObject, INavigationAware
    {
        public class AccentColorData
        {
            public string Name { get; set; }
            public Brush ColorBrush { get; set; }
        }

        private readonly ISettingsService _settingsService;
        private readonly IThemeSelectorService _themeSelectorService;

        private ApplicationTheme _theme;
        private SystemSettings _systemSettings;
        private ClipboardSettings _clipboardSettings;

        private ICommand _selectionChangedCommand;

        public SettingsViewModel(ISettingsService settingsService, IThemeSelectorService themeSelectorService)
        {
            _settingsService = settingsService;
            _themeSelectorService = themeSelectorService;
        }

        public ICommand SelectionChangedCommand => _selectionChangedCommand ??= new RelayCommand<string>(OnSelectionChanged);

        public List<AccentColorData> AccentColors { get; set; }

        public ApplicationTheme Theme
        {
            get => _theme;
            set => SetProperty(ref _theme, value);
        }

        public SystemSettings System
        {
            get => _systemSettings;
            set => SetProperty(ref _systemSettings, value);
        }

        public ClipboardSettings Clipboard
        {
            get => _clipboardSettings;
            set => SetProperty(ref _clipboardSettings, value);
        }

        public void OnNavigateTo(object parameter)
        {
            AccentColors = new List<AccentColorData>();
            AccentColors.Add(new AccentColorData { Name = "System", ColorBrush = SystemParameters.WindowGlassBrush });
            AccentColors.AddRange(ThemeManager.Current.Themes
                .GroupBy(x => x.ColorScheme)
                .OrderBy(a => a.Key)
                .Select(a => new AccentColorData { Name = a.Key, ColorBrush = a.First().ShowcaseBrush })
                .ToList());

            Theme = _settingsService.Get(Constants.SETTINGS_APPLICATION_THEME, ApplicationTheme.Default);
            System = _settingsService.Get(Constants.SETTINGS_SYSTEM, new SystemSettings());
            Clipboard = _settingsService.Get(Constants.SETTINGS_CLIPBOARD, new ClipboardSettings());
        }

        public void OnNavigateFrom()
        {
            _settingsService.Set(Constants.SETTINGS_SYSTEM, System);
            _settingsService.Set(Constants.SETTINGS_CLIPBOARD, Clipboard);
        }

        private void OnSelectionChanged(string parameter)
        {
            switch (parameter)
            {
                case "Theme":
                    _themeSelectorService.SetTheme(Theme);
                    break;
            }
        }
    }
}
