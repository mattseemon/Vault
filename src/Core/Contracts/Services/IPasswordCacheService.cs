using System.Security;

namespace Seemon.Vault.Core.Contracts.Services
{
    public interface IPasswordCacheService
    {
        SecureString Get(object key);

        void Set(object key, SecureString password, bool expires = false);

        void Clear(object key);
    }
}
