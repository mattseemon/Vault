using Org.BouncyCastle.Bcpg.OpenPgp;
using Seemon.Vault.Core.Contracts.Models;
using Seemon.Vault.Core.Models.OpenPGP;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IPGPService
    {
        SecureString GenerateKey(KeyGenParams keyGenerationParameters);

        Task<SecureString> GenerateKeyAsync(KeyGenParams keyGenerationParameters);

        SecureString GetAsciiArmored(PgpSecretKeyRing secretKeyRing);

        SecureString GetAsciiArmored(PgpPublicKeyRing publicKeyRing);

        SecureString ImportKeys(string path);

        void ExportPublicKeys(SecureString secureAsciiArmored, string exportPath, bool armor = true);

        void BackupSecretKeys(SecureString secureAsciiArmored, SecureString passphrase, string exportPath, bool armor = true);

        List<IEncryptionKeyRing> GetSecretKeyRings(Stream stream);

        List<IEncryptionKeyRing> GetPublicKeyRings(Stream stream);

        SecureString ChangePassphrase(SecureString secureAsciiArmored, SecureString oldPassphrase, SecureString newPassphrase);
    }
}
