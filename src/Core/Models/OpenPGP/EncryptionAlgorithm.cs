using System.ComponentModel;

namespace Seemon.Vault.Core.Models.OpenPGP
{
    public enum EncryptionAlgorithm
    {
        [Description("Rivest–Shamir–Adleman (RSA)")]
        RSA,
        [Description("Digital Signature Algorithm (DSA)")]
        DSA,
        [Description("Elliptic Curve Cryptography")]
        EC,
        [Description("ElGamal")]
        ELGAMAL
    }
}
