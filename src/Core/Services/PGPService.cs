using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Seemon.Vault.Core.Contracts.Models;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Exceptions;
using Seemon.Vault.Core.Helpers.Extensions;
using Seemon.Vault.Core.Models.OpenPGP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Seemon.Vault.Core.Services
{
    public class PGPService : IPGPService
    {
        public enum PGPOperation
        {
            Generate,
            Import,
            Export,
            ChangePassword
        }

        private readonly IApplicationInfoService _applicationInfoService;

        public PGPService(IApplicationInfoService applicationInfoService)
        {
            _applicationInfoService = applicationInfoService;
        }

        public SecureString GenerateKey(KeyGenParams keyGenerationParameters)
        {
            TimeSpan difference = TimeSpan.Zero;

            if (keyGenerationParameters.Expiry.HasValue)
            {
                difference = keyGenerationParameters.Expiry.Value.Subtract(DateTime.Today);
            }

            IAsymmetricCipherKeyPairGenerator generator = new RsaKeyPairGenerator();
            generator.Init(keyGenerationParameters.RsaParameters);

            DateTime created = DateTime.UtcNow;

            PgpKeyPair masterKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.RsaGeneral, generator.GenerateKeyPair(), created);

            PgpSignatureSubpacketGenerator masterSubpacketGenerator = new PgpSignatureSubpacketGenerator();
            masterSubpacketGenerator.SetKeyFlags(true, (int)(EncryptionKeyFlags.Sign | EncryptionKeyFlags.Certify | EncryptionKeyFlags.Authenticate));
            masterSubpacketGenerator.SetPreferredSymmetricAlgorithms(true, (from a in keyGenerationParameters.KeyAlgorithms select (int)a).ToArray());
            masterSubpacketGenerator.SetPreferredCompressionAlgorithms(true, (from a in keyGenerationParameters.CompressionAlgorithms select (int)a).ToArray());
            masterSubpacketGenerator.SetPreferredHashAlgorithms(true, (from a in keyGenerationParameters.HashAlgorithms select (int)a).ToArray());
            if (keyGenerationParameters.Expiry.HasValue)
            {
                masterSubpacketGenerator.SetKeyExpirationTime(true, Convert.ToInt32(difference.TotalSeconds));
            }

            PgpKeyPair encryptionKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.RsaGeneral, generator.GenerateKeyPair(), created);

            PgpSignatureSubpacketGenerator encryptionSubpacketGenerator = new PgpSignatureSubpacketGenerator();
            encryptionSubpacketGenerator.SetKeyFlags(true, (int)(EncryptionKeyFlags.Encrypt));
            encryptionSubpacketGenerator.SetPreferredSymmetricAlgorithms(true, (from a in keyGenerationParameters.KeyAlgorithms select (int)a).ToArray());
            encryptionSubpacketGenerator.SetPreferredCompressionAlgorithms(true, (from a in keyGenerationParameters.CompressionAlgorithms select (int)a).ToArray());
            encryptionSubpacketGenerator.SetPreferredHashAlgorithms(true, (from a in keyGenerationParameters.HashAlgorithms select (int)a).ToArray());
            if (keyGenerationParameters.Expiry.HasValue)
            {
                encryptionSubpacketGenerator.SetKeyExpirationTime(true, Convert.ToInt32(difference.TotalSeconds));
            }

            var identity = keyGenerationParameters.Name;
            if (!string.IsNullOrEmpty(keyGenerationParameters.Comment))
            {
                identity = $"{identity} ({keyGenerationParameters.Comment})";
            }
            if (!string.IsNullOrEmpty(keyGenerationParameters.Email))
            {
                identity = $"{identity} <{keyGenerationParameters.Email}>";
            }

            var passPhrase = keyGenerationParameters.Passphrase.ToUnsecuredString().ToCharArray();

            PgpKeyRingGenerator keyRingGenerator = new PgpKeyRingGenerator(PgpSignature.DefaultCertification, masterKeyPair, identity, SymmetricKeyAlgorithmTag.Aes256, passPhrase, true, masterSubpacketGenerator.Generate(), null, new SecureRandom());
            keyRingGenerator.AddSubKey(encryptionKeyPair, encryptionSubpacketGenerator.Generate(), null);

            return GetAsciiArmored(keyRingGenerator.GenerateSecretKeyRing());
        }

        public async Task<SecureString> GenerateKeyAsync(KeyGenParams keyGenerationParameters) => await Task.Run(() => GenerateKey(keyGenerationParameters));

        public SecureString GetAsciiArmored(PgpSecretKeyRing secretKeyRing)
        {
            using var stream = new MemoryStream();
            var armoredStream = new ArmoredOutputStream(stream);
            SetHeaders(armoredStream);

            secretKeyRing.Encode(armoredStream);
            armoredStream.Dispose();

            var armored = Encoding.ASCII.GetString(stream.ToArray());
            return armored.ToSecureString();
        }

        public SecureString GetAsciiArmored(PgpPublicKeyRing publicKeyRing)
        {
            using var stream = new MemoryStream();
            var armoredStream = new ArmoredOutputStream(stream);
            SetHeaders(armoredStream);

            publicKeyRing.Encode(armoredStream);
            armoredStream.Dispose();

            var armored = Encoding.ASCII.GetString(stream.ToArray());
            return armored.ToSecureString();
        }

        public List<IEncryptionKeyRing> GetPublicKeyRings(Stream stream)
        {
            var keyRings = new List<IEncryptionKeyRing>();

            using var inputStream = PgpUtilities.GetDecoderStream(stream) as ArmoredInputStream;
            try
            {
                var keyRingBundle = new PgpPublicKeyRingBundle(inputStream);
                foreach (var keyRing in keyRingBundle.GetKeyRings().Cast<PgpPublicKeyRing>())
                {
                    keyRings.Add(new EncryptionKeyRing(keyRing));
                }
            }
            catch (PgpException) { }
            return keyRings;
        }

        public List<IEncryptionKeyRing> GetSecretKeyRings(Stream stream)
        {
            var keyRings = new List<IEncryptionKeyRing>();

            using var inputStream = PgpUtilities.GetDecoderStream(stream) as ArmoredInputStream;
            try
            {
                var keyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                foreach (var keyRing in keyRingBundle.GetKeyRings().Cast<PgpSecretKeyRing>())
                {
                    keyRings.Add(new EncryptionKeyRing(keyRing));
                }
            }
            catch (PgpException) { }
            return keyRings;
        }

        public SecureString ImportKeys(string path)
        {
            try
            {
                using var stream = File.OpenRead(path);
                using var inputStream = PgpUtilities.GetDecoderStream(stream);

                using var ms = new MemoryStream();
                var armoredStream = new ArmoredOutputStream(ms);
                SetHeaders(armoredStream, PGPOperation.Import, path);
                inputStream.CopyTo(armoredStream);
                armoredStream.Dispose();

                return Encoding.ASCII.GetString(ms.ToArray()).ToSecureString();
            }
            catch (Exception ex)
            {
                throw new VaultException("Error occured while importing PGP key pair.", ex);
            }
        }

        public void ExportPublicKeys(SecureString secureAsciiArmored, string exportPath, bool armor = true)
        {
            PgpPublicKeyRing publicKeyRing = null;
            var asciiArmored = secureAsciiArmored.ToUnsecuredString();
            var inputStream = PgpUtilities.GetDecoderStream(asciiArmored.GetStream());

            try
            {
                foreach (var secretKeyRing in new PgpSecretKeyRingBundle(inputStream).GetKeyRings().Cast<PgpSecretKeyRing>())
                {
                    foreach (var secretKey in secretKeyRing.GetSecretKeys().Cast<PgpSecretKey>())
                    {
                        publicKeyRing = publicKeyRing is null
                            ? new PgpPublicKeyRing(secretKey.PublicKey.GetEncoded())
                            : PgpPublicKeyRing.InsertPublicKey(publicKeyRing, secretKey.PublicKey);
                    }
                }
            }
            catch
            {
                if (inputStream is null)
                {
                    inputStream.Dispose();
                }
                inputStream = PgpUtilities.GetDecoderStream(asciiArmored.GetStream());
                var publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);
                publicKeyRing = publicKeyRingBundle.GetKeyRings().Cast<PgpPublicKeyRing>().FirstOrDefault();
            }

            if (publicKeyRing is null)
            {
                return;
            }

            var extension = armor ? "asc" : "gpg";
            var keyId = publicKeyRing.GetPublicKeys().Cast<PgpPublicKey>().FirstOrDefault(x => x.IsMasterKey).KeyId;
            var publicKeyPath = Path.Combine(exportPath, $"0x{keyId.GetKeyIdHex(true).Replace(" ", "")}.public.{extension}");

            Stream outPutStream = null;
            using (var stream = File.Open(publicKeyPath, FileMode.Create, FileAccess.Write))
            {
                outPutStream = stream;
                if (armor)
                {
                    var armoredStream = new ArmoredOutputStream(stream);
                    SetHeaders(armoredStream, PGPOperation.Export);
                    outPutStream = armoredStream;
                }

                publicKeyRing.Encode(outPutStream);
                outPutStream.Dispose();
            }
        }

        public void BackupSecretKeys(SecureString secureAsciiArmored, SecureString passphrase, string exportPath, bool armor = true)
        {
            var passphraseArray = passphrase.ToUnsecuredString().ToCharArray();
            var asciiArmored = secureAsciiArmored.ToUnsecuredString();
            var inputStream = PgpUtilities.GetDecoderStream(asciiArmored.GetStream());

            var secrectKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
            var secretKeyRing = secrectKeyRingBundle.GetKeyRings().Cast<PgpSecretKeyRing>().FirstOrDefault();

            if (secretKeyRing is null)
            {
                return;
            }

            try
            {
                var masterKey = secretKeyRing.GetSecretKeys().Cast<PgpSecretKey>().FirstOrDefault(x => x.IsMasterKey);
                masterKey.ExtractPrivateKey(passphraseArray);

                var extension = armor ? "asc" : "gpg";
                var keyId = masterKey.KeyId;
                var backupKeyPath = Path.Combine(exportPath, $"0x{keyId.GetKeyIdHex(true).Replace(" ", "")}.private.{extension}");

                Stream outputStream = null;
                using (var stream = File.Open(backupKeyPath, FileMode.Create, FileAccess.Write))
                {
                    outputStream = stream;
                    if (armor)
                    {
                        var armoredStream = new ArmoredOutputStream(stream);
                        SetHeaders(armoredStream, PGPOperation.Export);
                        outputStream = armoredStream;
                    }

                    secretKeyRing.Encode(outputStream);
                    outputStream.Dispose();
                }
            }
            catch { throw new VaultException("PGP key pair - Backup secret key failed."); }
        }

        public SecureString ChangePassphrase(SecureString secureAsciiArmored, SecureString oldPassphrase, SecureString newPassphrase)
        {
            try
            {
                using var stream = new MemoryStream();
                var armoredOutputStream = new ArmoredOutputStream(stream);
                SetHeaders(armoredOutputStream);

                var inputStream = secureAsciiArmored.ToUnsecuredString().GetStream();
                var keyRing = new PgpSecretKeyRing(PgpUtilities.GetDecoderStream(inputStream));
                var tempKeyRing = new PgpSecretKeyRing(keyRing.GetEncoded());

                var oldPassphraseArray = oldPassphrase.ToUnsecuredString().ToCharArray();
                var newPassphraseArray = newPassphrase.ToUnsecuredString().ToCharArray();

                foreach (var secretKey in keyRing.GetSecretKeys().Cast<PgpSecretKey>())
                {
                    var tempSecretKey = PgpSecretKey.CopyWithNewPassword(secretKey, oldPassphraseArray, newPassphraseArray, secretKey.KeyEncryptionAlgorithm, new SecureRandom());
                    tempKeyRing = PgpSecretKeyRing.RemoveSecretKey(tempKeyRing, secretKey);
                    tempKeyRing = PgpSecretKeyRing.InsertSecretKey(tempKeyRing, tempSecretKey);
                }

                tempKeyRing.Encode(armoredOutputStream);
                armoredOutputStream.Dispose();

                return Encoding.ASCII.GetString(stream.ToArray()).ToSecureString();
            }
            catch (Exception ex)
            {
                throw new VaultException("PGP key pair - change passphrase failed.", ex);
            }
        }

        private void SetHeaders(ArmoredOutputStream armoredStream, PGPOperation operation = PGPOperation.Generate, params string[] additional)
        {
            switch (operation)
            {
                case PGPOperation.Generate:
                    armoredStream.AddHeader("Comment", $"Generated By {_applicationInfoService.GetTitle()} - v{_applicationInfoService.GetVersion()}");
                    armoredStream.AddHeader("Comment", $"Generated at {DateTime.UtcNow:r}");
                    break;
                case PGPOperation.Import:
                    if (additional.Any())
                    {
                        armoredStream.AddHeader("Comment", $"Imported from file {additional[0]}");
                    }
                    armoredStream.AddHeader("Comment", $"Imported at {DateTime.UtcNow:r}");
                    break;
                case PGPOperation.Export:
                    armoredStream.AddHeader("Comment", $"Exported By {_applicationInfoService.GetTitle()} - v{_applicationInfoService.GetVersion()}");
                    armoredStream.AddHeader("Comment", $"Exported at {DateTime.UtcNow:r}");
                    break;
            }
        }
    }
}
