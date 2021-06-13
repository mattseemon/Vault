using System.ComponentModel;

namespace Seemon.Vault.Core.Models.OpenPGP
{
    public enum SymmetricKeyAlgorithm
    {
        [Description("NULL")]
        Null = 0,
        [Description("IDEA")]
        Idea = 1,
        [Description("TRIPLE-DES")]
        TripleDes = 2,
        [Description("CAST-5")]
        Cast5 = 3,
        [Description("BLOWFISH")]
        Blowfish = 4,
        [Description("SAFER")]
        Safer = 5,
        [Description("DES")]
        Des = 6,
        [Description("AES-128")]
        Aes128 = 7,
        [Description("AES-192")]
        Aes192 = 8,
        [Description("AES-256")]
        Aes256 = 9,
        [Description("TWOFISH")]
        Twofish = 10,
        [Description("CAMELLIA-128")]
        Camellia128 = 11,
        [Description("CAMELLIA-192")]
        Camellia192 = 12,
        [Description("CAMELLIA-256")]
        Camellia256 = 13
    }
}
