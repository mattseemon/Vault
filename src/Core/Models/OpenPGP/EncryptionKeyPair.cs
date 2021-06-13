using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Utilities.Encoders;
using Seemon.Vault.Core.Contracts.Models;
using Seemon.Vault.Core.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Seemon.Vault.Core.Models.OpenPGP
{
    public class EncryptionKeyPair : IEncryptionKeyPair
    {
        private readonly PgpSecretKey _secretKey;
        private readonly PgpPublicKey _publicKey;

        public EncryptionKeyPair() => Users = new List<string>();

        public EncryptionKeyPair(PgpSecretKey key)
            : this(key.PublicKey)
        {
            _secretKey = key;

            IsSigningKey = _secretKey.IsSigningKey;
            HasPrivateKey = !_secretKey.IsPrivateKeyEmpty;
        }

        public EncryptionKeyPair(PgpPublicKey key)
            : this()
        {
            _publicKey = key;
            LoadPublicKey();
        }

        public EncryptionAlgorithm Algorithm { get; private set; }

        public DateTime Created { get; private set; }

        public DateTime? Expiry { get; private set; }

        public bool IsMasterKey { get; private set; }

        public bool IsEncryptionKey { get; private set; }

        public bool IsSigningKey { get; private set; }

        public string Fingerprint { get; private set; }

        public int KeyFlags { get; private set; }

        public EncryptionKeyFlags Usage => (EncryptionKeyFlags)KeyFlags;

        public bool CanSign => ((EncryptionKeyFlags)KeyFlags & EncryptionKeyFlags.Sign) == EncryptionKeyFlags.Sign;

        public bool CanCertify => ((EncryptionKeyFlags)KeyFlags & EncryptionKeyFlags.Certify) == EncryptionKeyFlags.Certify;

        public bool CanEncryptCommunications => ((EncryptionKeyFlags)KeyFlags & EncryptionKeyFlags.EncryptCommunications) == EncryptionKeyFlags.EncryptCommunications;

        public bool CanEncryptStorage => ((EncryptionKeyFlags)KeyFlags & EncryptionKeyFlags.EncryptStorage) == EncryptionKeyFlags.EncryptStorage;

        public bool CanAuthenticate => ((EncryptionKeyFlags)KeyFlags & EncryptionKeyFlags.Authenticate) == EncryptionKeyFlags.Authenticate;

        public bool HasPrivateKey { get; private set; }

        public bool HasExpired => Expiry.HasValue && DateTime.Now > Expiry.Value;

        public bool HasNoExpiry => !Expiry.HasValue;

        public long KeyId { get; private set; }

        public int KeySize { get; private set; }

        public CompressionAlgorithm[] PreferredCompressions { get; private set; }

        public SymmetricKeyAlgorithm[] PreferredCiphers { get; private set; }

        public HashAlgorithm[] PreferredHashes { get; private set; }

        public string User => Users.FirstOrDefault();

        public List<string> Users { get; }

        private void LoadPublicKey()
        {
            KeyId = _publicKey.KeyId;
            IsMasterKey = _publicKey.IsMasterKey;

            Fingerprint = Hex.ToHexString(_publicKey.GetFingerprint()).GetFormattedHex();
            IsEncryptionKey = _publicKey.IsEncryptionKey;
            KeySize = _publicKey.BitStrength;

            Users.Clear();
            foreach (string userid in _publicKey.GetUserIds())
            {
                Users.Add(userid);
            }

            switch (_publicKey.Algorithm)
            {
                case PublicKeyAlgorithmTag.RsaGeneral:
                case PublicKeyAlgorithmTag.RsaEncrypt:
                case PublicKeyAlgorithmTag.RsaSign:
                    Algorithm = EncryptionAlgorithm.RSA;
                    break;
                case PublicKeyAlgorithmTag.ElGamalGeneral:
                case PublicKeyAlgorithmTag.ElGamalEncrypt:
                    Algorithm = EncryptionAlgorithm.ELGAMAL;
                    break;
                case PublicKeyAlgorithmTag.Dsa:
                    Algorithm = EncryptionAlgorithm.DSA;
                    break;
                case PublicKeyAlgorithmTag.ECDH:
                case PublicKeyAlgorithmTag.ECDsa:
                case PublicKeyAlgorithmTag.EdDsa:
                    Algorithm = EncryptionAlgorithm.EC;
                    break;
            }

            Created = _publicKey.CreationTime;
            var seconds = _publicKey.GetValidSeconds();
            if (seconds > 0)
            {
                Expiry = _publicKey.CreationTime.AddSeconds(seconds);
            }

            foreach (PgpSignature signature in _publicKey.GetSignatures())
            {
                if (signature.HasSubpackets)
                {
                    var subPackets = signature.GetHashedSubPackets();

                    var compressions = subPackets.GetPreferredCompressionAlgorithms();
                    if (compressions is not null)
                    {
                        PreferredCompressions = compressions.Select(x => (CompressionAlgorithm)x).ToArray();
                    }
                    var hashes = subPackets.GetPreferredHashAlgorithms();
                    if (hashes is not null)
                    {
                        PreferredHashes = hashes.Select(x => (HashAlgorithm)x).ToArray();
                    }
                    var ciphers = subPackets.GetPreferredSymmetricAlgorithms();
                    if (ciphers is not null)
                    {
                        PreferredCiphers = ciphers.Select(x => (SymmetricKeyAlgorithm)x).ToArray();
                    }
                    var flags = subPackets.GetKeyFlags();
                    if (flags != 0)
                    {
                        KeyFlags = flags;
                    }
                }
            }
        }
    }
}
