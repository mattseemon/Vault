using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Seemon.Vault.Core.Helpers.Services;
using System;
using System.Security;

namespace Seemon.Vault.Core.Models.OpenPGP
{
    public class KeyGenParams
    {
        public KeyGenParams(int keySize, SecureString passphrase, string name, string email, string comment, DateTime? expiry = null)
        {
            KeyAlgorithms = new SymmetricKeyAlgorithm[]
            {
                SymmetricKeyAlgorithm.Aes256,
                SymmetricKeyAlgorithm.Aes192,
                SymmetricKeyAlgorithm.Aes128,
                SymmetricKeyAlgorithm.TripleDes
            };

            HashAlgorithms = new HashAlgorithm[]
            {
                HashAlgorithm.Sha512,
                HashAlgorithm.Sha384,
                HashAlgorithm.Sha256,
                HashAlgorithm.Sha224,
                HashAlgorithm.Sha1
            };

            CompressionAlgorithms = new CompressionAlgorithm[]
            {
                CompressionAlgorithm.Zip,
                CompressionAlgorithm.ZLib,
                CompressionAlgorithm.BZip2
            };

            var keyStrength = PGPHelper.GetRSAStrength(keySize);

            RsaParameters = new RsaKeyGenerationParameters(BigInteger.ValueOf(0x10001), new SecureRandom(), keySize, keyStrength);
            Passphrase = passphrase;
            Name = name;
            Email = email;
            Comment = comment;
            Expiry = expiry;
        }

        public SymmetricKeyAlgorithm[] KeyAlgorithms { get; private set; }

        public HashAlgorithm[] HashAlgorithms { get; private set; }

        public CompressionAlgorithm[] CompressionAlgorithms { get; private set; }

        public RsaKeyGenerationParameters RsaParameters { get; private set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public string Comment { get; private set; }

        public DateTime? Expiry { get; private set; }

        public SecureString Passphrase { get; private set; }
    }
}
