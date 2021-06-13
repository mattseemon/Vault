using Seemon.Vault.Core.Models.Security;
using System.Collections.Generic;
using System.Security;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IPasswordService
    {
        SecureString CreatePassword(string key);

        SecureString GetPassword(string key, int attempt = 0, bool forcePrompt = false);

        ChangePasswordResponse ChangePassword(string key, List<PasswordHash> exclusionList = null);

        PasswordHash HashPassword(SecureString password);

        bool VerifyPassword(SecureString password, PasswordHash verificationHash);

        void ClearPassword(string key);
    }
}
