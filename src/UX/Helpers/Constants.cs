using System.Windows.Media;

namespace Seemon.Vault.Helpers
{
    public static class Constants
    {
        internal static string SETTINGS_APPLICATION_THEME = "application.theme";
        internal static string SETTINGS_SYSTEM = "settings.system";
        internal static string SETTINGS_CLIPBOARD = "settings.clipboard";
        internal static string SETTINGS_UDPATES = "settings.updates";
        internal static string SETTINGS_PGP = "settings.pgp";
        internal static string SETTINGS_GIT = "settings.git";
        internal static string SETTINGS_PASSWORD_GENERATOR = "settings.password.generator";
        internal static string SETTINGS_SECURITY = "settings.security";
        internal static string SETTINGS_PROFILES = "settings.profiles";
        internal static string SETTINGS_WINDOWS = "settings.windows";

        internal static SolidColorBrush NOTIFICATION_COLOR_INFO = new((Color)ColorConverter.ConvertFromString("#0d47a1"));
        internal static SolidColorBrush NOTIFICATION_COLOR_WARN = new((Color)ColorConverter.ConvertFromString("#FF8800"));
        internal static SolidColorBrush NOTIFICATION_COLOR_ERROR = new((Color)ColorConverter.ConvertFromString("#CC0000"));
        internal static SolidColorBrush NOTIFICATION_COLOR_UPDATE = new((Color)ColorConverter.ConvertFromString("#007E33"));
    }
}
