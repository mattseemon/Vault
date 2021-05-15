using ControlzEx.Theming;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Models;
using Seemon.Vault.Helpers;
using System.Windows;

namespace Seemon.Vault.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private readonly ISettingsService _settingsService;

        public ThemeSelectorService(ISettingsService settingsService) => _settingsService = settingsService;

        public void InitializeTheme()
        {
            var theme = GetCurrentTheme();
            SetTheme(theme);
        }

        public ApplicationTheme GetCurrentTheme()
        {
            return _settingsService.Get<ApplicationTheme>(Constants.SETTINGS_APPLICATION_THEME, ApplicationTheme.Default);
        }

        public void SetTheme(ApplicationTheme theme)
        {
            if (theme == ApplicationTheme.Default)
            {
                ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncAll);
            }
            else
            {
                ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncAll);
                if (theme.Base == ApplicationTheme.ThemeBase.System)
                {
                    ThemeManager.Current.ChangeThemeColorScheme(Application.Current, theme.Accent);
                }
                else if (theme.Accent == "System")
                {
                    ThemeManager.Current.ChangeThemeBaseColor(Application.Current, theme.Base.ToString());
                }
                else
                {
                    ThemeManager.Current.ChangeTheme(Application.Current, theme.ToString(), SystemParameters.HighContrast);
                }
            }
            _settingsService.Set(Constants.SETTINGS_APPLICATION_THEME, theme);
        }
    }
}
