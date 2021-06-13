using Org.BouncyCastle.Bcpg.OpenPgp;
using Seemon.Vault.Core.Contracts.Models;
using Seemon.Vault.Core.Helpers.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Seemon.Vault.Core.Models.OpenPGP
{
    public class EncryptionKeyRing : IEncryptionKeyRing
    {
        private SecureString _asciiArmored;

        public List<IEncryptionKeyPair> KeyPairs { get; }

        public PgpSecretKeyRing SecretKeyRing { get; }

        public PgpPublicKeyRing PublicKeyRing { get; }

        public long KeyId => MasterKeyPair.KeyId;

        public IEncryptionKeyPair MasterKeyPair => KeyPairs.FirstOrDefault(x => x.IsMasterKey);

        public EncryptionKeyRing() => KeyPairs = new List<IEncryptionKeyPair>();

        public EncryptionKeyRing(SecureString asciiArmored)
            : this()
        {
            _asciiArmored = asciiArmored;

            try
            {
                var stream = _asciiArmored.ToUnsecuredString().GetStream();
                using var inputStream = PgpUtilities.GetDecoderStream(stream);
                var secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                SecretKeyRing = secretKeyRingBundle.GetKeyRings()
                    .Cast<PgpSecretKeyRing>()
                    .FirstOrDefault();

                LoadSecretKeys();
            }
            catch
            {
                var stream = _asciiArmored.ToUnsecuredString().GetStream();
                using var inputStream = PgpUtilities.GetDecoderStream(stream);
                var publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);
                PublicKeyRing = publicKeyRingBundle.GetKeyRings()
                    .Cast<PgpPublicKeyRing>()
                    .FirstOrDefault();

                LoadPublicKeys();
            }
        }

        public EncryptionKeyRing(PgpPublicKeyRing keyRing)
            : this()
        {
            PublicKeyRing = keyRing;
            LoadPublicKeys();
        }

        public EncryptionKeyRing(PgpSecretKeyRing keyRing)
            : this()
        {
            SecretKeyRing = keyRing;
            LoadSecretKeys();
        }

        private void LoadSecretKeys()
        {
            KeyPairs.Clear();
            foreach (var secretKey in SecretKeyRing.GetSecretKeys().Cast<PgpSecretKey>())
            {
                KeyPairs.Add(new EncryptionKeyPair(secretKey));
            }
        }

        private void LoadPublicKeys()
        {
            KeyPairs.Clear();
            foreach (var publicKey in PublicKeyRing.GetPublicKeys().Cast<PgpPublicKey>())
            {
                KeyPairs.Add(new EncryptionKeyPair(publicKey));
            }
        }
    }
}
