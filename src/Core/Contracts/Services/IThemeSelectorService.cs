using Seemon.Vault.Core.Models;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IThemeSelectorService
    {
        void InitializeTheme();

        void SetTheme(ApplicationTheme theme);

        ApplicationTheme GetCurrentTheme();
    }
}
