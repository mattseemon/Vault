using System.ComponentModel;

namespace Seemon.Vault.Core.Models.OpenPGP
{
    public enum HashAlgorithm
    {
        [Description("MD5")]
        MD5 = 1,
        [Description("SHA-1")]
        Sha1 = 2,
        [Description("RIPEMD-160")]
        RipeMD160 = 3,
        [Description("DOUBLE-SHA")]
        DoubleSha = 4,
        [Description("MD2")]
        MD2 = 5,
        [Description("TIGER-192")]
        Tiger192 = 6,
        [Description("HAVAL-5-PASS-160")]
        Haval5pass160 = 7,
        [Description("SHA-256")]
        Sha256 = 8,
        [Description("SHA-384")]
        Sha384 = 9,
        [Description("SHA-512")]
        Sha512 = 10,
        [Description("SHA-224")]
        Sha224 = 11
    }
}
