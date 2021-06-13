using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Collections.Generic;

namespace Seemon.Vault.Core.Contracts.Models
{
    public interface IEncryptionKeyRing
    {
        long KeyId { get; }

        List<IEncryptionKeyPair> KeyPairs { get; }

        IEncryptionKeyPair MasterKeyPair { get; }

        PgpSecretKeyRing SecretKeyRing { get; }

        PgpPublicKeyRing PublicKeyRing { get; }
    }
}
