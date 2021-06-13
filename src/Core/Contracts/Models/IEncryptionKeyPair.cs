using Seemon.Vault.Core.Models.OpenPGP;
using System;
using System.Collections.Generic;

namespace Seemon.Vault.Core.Contracts.Models
{
    public interface IEncryptionKeyPair
    {
        long KeyId { get; }

        int KeySize { get; }
        string Fingerprint { get; }

        EncryptionAlgorithm Algorithm { get; }

        DateTime Created { get; }

        DateTime? Expiry { get; }

        bool IsMasterKey { get; }

        bool IsSigningKey { get; }

        bool IsEncryptionKey { get; }

        bool HasPrivateKey { get; }

        int KeyFlags { get; }

        EncryptionKeyFlags Usage { get; }

        bool CanSign { get; }

        bool CanCertify { get; }

        bool CanEncryptCommunications { get; }

        bool CanEncryptStorage { get; }

        bool CanAuthenticate { get; }
        
        bool HasExpired { get; }

        bool HasNoExpiry { get; }

        SymmetricKeyAlgorithm[] PreferredCiphers { get; }

        CompressionAlgorithm[] PreferredCompressions { get; }

        HashAlgorithm[] PreferredHashes { get; }

        string User { get; }

        List<string> Users { get; }
    }
}
