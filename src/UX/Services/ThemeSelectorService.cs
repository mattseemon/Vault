using ControlzEx.Theming;
using Microsoft.Extensions.Logging;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models;
using System.Windows;
using System.Windows.Media;

namespace Seemon.Vault.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogger<IThemeSelectorService> _logger;

        public ThemeSelectorService(ISettingsService settingsService, ILogger<IThemeSelectorService> logger)
        {
            _settingsService = settingsService;
            _logger = logger;
        }

        public void InitializeTheme() => SetTheme(GetCurrentTheme());

        public ApplicationTheme GetCurrentTheme() => _settingsService.Get(Constants.SETTINGS_APPLICATION_THEME, ApplicationTheme.Default);

        public void SetTheme(ApplicationTheme theme)
        {
            _logger.LogInformation($"Setting application theme to {theme}");
            if (theme.ToString() == ApplicationTheme.Default.ToString())
            {
                ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncAll);
            }
            else
            {
                ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncAll);
                if (theme.Base == ApplicationTheme.ThemeBase.System)
                {
                    var currentTheme = ThemeManager.Current.DetectTheme(Application.Current);
                    var inverseThem = ThemeManager.Current.GetInverseTheme(currentTheme);

                    ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme(inverseThem.BaseColorScheme, (Color)ColorConverter.ConvertFromString(theme.Accent)));
                    ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme(currentTheme.BaseColorScheme, (Color)ColorConverter.ConvertFromString(theme.Accent))));
                }
                else if (theme.Accent == "System")
                {
                    ThemeManager.Current.ChangeThemeBaseColor(Application.Current, theme.Base.ToString());
                }
                else
                {
                    var newTheme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(theme.Base.ToString(), (Color)ColorConverter.ConvertFromString(theme.Accent));
                    var inverseTheme = ThemeManager.Current.GetInverseTheme(newTheme);

                    ThemeManager.Current.AddTheme(inverseTheme);
                    ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.AddTheme(newTheme));
                }
            }
            _settingsService.Set(Constants.SETTINGS_APPLICATION_THEME, theme);
        }
    }
}
