using System.Security;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IEncryptionService
    {
        string Encrypt(string value, SecureString password);

        string Decrypt(string encryptedValue, SecureString password);
    }
}
