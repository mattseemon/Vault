using MahApps.Metro.Controls.Dialogs;
using Seemon.Vault.Core.Contracts.ViewModels;
using Seemon.Vault.Core.Contracts.Views;
using Seemon.Vault.Helpers.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Seemon.Vault.Helpers.Extensions
{
    public static class WindowExtensions
    {
        private static readonly MetroDialogSettings dialogSettings = new()
        {
            ColorScheme = MetroDialogColorScheme.Accented,
            AffirmativeButtonText = "OK",
            NegativeButtonText = "Cancel"
        };

        public static IViewModel GetDataContext(this Window window)
            => (IViewModel)(window.FindName("ShellFrame") is Frame frame
                ? frame.GetDataContext()
                : window.DataContext ?? null);

        public static async Task<ProgressDialogController> ShowProgressPromptAsync(this IWindow window, string title, string message)
            => await ((WindowBase)window).ShowProgressAsync(title, message, settings: dialogSettings);

        public static async Task<MessageDialogResult> ShowMessagePromptAsync(this IWindow window, string title, string message, MessageDialogStyle style, MetroDialogSettings settings = null)
            => await ((WindowBase)window).ShowMessageAsync(title, message, style, settings ?? dialogSettings);
    }
}
