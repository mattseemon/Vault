using System.Security;

namespace Seemon.Vault.Core.Models.Security
{
    public class ChangePasswordResponse
    {
        public SecureString OldPassword { get; set; }

        public SecureString NewPassword { get; set; }
    }
}
