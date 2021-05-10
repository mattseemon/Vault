using Seemon.Vault.Core.Contracts.Services;
using System.Diagnostics;

namespace Seemon.Vault.Core.Services
{
    public class SystemService : ISystemService
    {
        public void OpenInWebBrowser(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }
    }
}
