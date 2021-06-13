using System.Windows.Media;

namespace Seemon.Vault.Core.Helpers
{
    public static class Constants
    {
        public static readonly string SETTINGS_APPLICATION_THEME = "application.theme";
        public static readonly string SETTINGS_SYSTEM = "settings.system";
        public static readonly string SETTINGS_CLIPBOARD = "settings.clipboard";
        public static readonly string SETTINGS_UDPATES = "settings.updates";
        public static readonly string SETTINGS_PGP = "settings.pgp";
        public static readonly string SETTINGS_GIT = "settings.git";
        public static readonly string SETTINGS_PASSWORD_GENERATOR = "settings.password.generator";
        public static readonly string SETTINGS_SECURITY = "settings.security";
        public static readonly string SETTINGS_PROFILES = "settings.profiles";
        public static readonly string SETTINGS_WINDOWS = "settings.windows";

        public static readonly string PASSWORD_CACHE_KEY_STORE = "PASSWORD_CACHE_KEY_STORE";

        public static readonly SolidColorBrush NOTIFICATION_COLOR_INFO = new((Color)ColorConverter.ConvertFromString("#0099CC"));
        public static readonly SolidColorBrush NOTIFICATION_COLOR_WARN = new((Color)ColorConverter.ConvertFromString("#FF8800"));
        public static readonly SolidColorBrush NOTIFICATION_COLOR_ERROR = new((Color)ColorConverter.ConvertFromString("#CC0000"));
        public static readonly SolidColorBrush NOTIFICATION_COLOR_UPDATE = new((Color)ColorConverter.ConvertFromString("#00695C"));
        public static readonly SolidColorBrush NOTIFICATION_COLOR_SUCCESS = new((Color)ColorConverter.ConvertFromString("#007E33"));
    }
}
