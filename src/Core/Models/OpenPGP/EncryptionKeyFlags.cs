using System;

namespace Seemon.Vault.Core.Models.OpenPGP
{
    [Flags]
    public enum EncryptionKeyFlags : int
    {
        Certify = 0x01,
        Sign = 0x02,
        EncryptCommunications = 0x04,
        EncryptStorage = 0x08,
        Split = 0x10,
        Encrypt = EncryptCommunications | EncryptStorage,
        Authenticate = 0x20,
        Shared = 0x80
    }
}
