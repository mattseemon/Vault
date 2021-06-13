using Seemon.Vault.Core.Models.KeyStore;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IKeyStoreService
    {
        bool InitializeKeyStore(string path);

        bool VerifyPassword(SecureString password);

        bool ChangePassword();

        Task<bool> ChangePasswordAsync();

        void InsertKeyPair(Guid id, KeyPairInfo keyPair, SecureString asciiArmored);

        void UpdateKeyPair(Guid id, KeyPairInfo keyPair, SecureString asciiArmored);

        void DeleteKeyPair(Guid id);

        void DeleteKeyPairs(List<Guid> ids);

        bool SetDefaultKeyPair(Guid id);

        KeyPairInfo GetKeyPair(Guid id);

        SecureString GetKeyPairAscii(Guid id);

        List<KeyPairInfo> GetKeyPairs();

        Task<List<KeyPairInfo>> GetKeyPairsAsync();
    }
}
