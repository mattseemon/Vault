using System.ComponentModel;

namespace Seemon.Vault.Core.Models.OpenPGP
{
    public enum CompressionAlgorithm
    {
        [Description("Uncompressed")]
        Uncompressed = 0,
        [Description("ZIP")]
        Zip = 1,
        [Description("ZLIB")]
        ZLib = 2,
        [Description("BZIP2")]
        BZip2 = 3
    }
}
