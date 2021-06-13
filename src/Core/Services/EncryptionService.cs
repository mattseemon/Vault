/************************************************************************************
* Author: Tom Rucki (https://tomrucki.com/)                                         *
* Availability: https://tomrucki.com/posts/aes-encryption-in-csharp/                *
************************************************************************************/

using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Helpers.Services;
using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Seemon.Vault.Core.Services
{
    public class EncryptionService : IEncryptionService
    {
        private const int _aesBlockByteSize = 128 / 8;

        private const int _passwordSaltByteSize = 256 / 8;
        private const int _passwordByteSize = 256 / 8;
        private const int _passwordIterationCount = 100000;

        private const int _signatureByteSize = 256 / 8;

        private const int _minimumEncryptedMessageByteSize =
            _passwordSaltByteSize +
            _passwordSaltByteSize +
            _aesBlockByteSize +
            _aesBlockByteSize +
            _signatureByteSize;

        private static readonly Encoding _stringEncoding = Encoding.UTF8;
        internal static ThreadLocal<RandomNumberGenerator> _random = new(() => new RNGCryptoServiceProvider());

        public string Encrypt(string value, SecureString password)
        {
            // Encrypt
            var keySalt = EncryptionHelper.GenerateRandomBytes(_passwordSaltByteSize);
            var key = EncryptionHelper.GenerateKey(password, keySalt, _passwordByteSize, _passwordIterationCount);
            var iv = EncryptionHelper.GenerateRandomBytes(_aesBlockByteSize);

            byte[] cipherText;

            using (var aes = EncryptionHelper.CreateAes())
            {
                using var encryptor = aes.CreateEncryptor(key, iv);
                var plainText = _stringEncoding.GetBytes(value);
                cipherText = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
            }

            // Sign
            var authKeySalt = EncryptionHelper.GenerateRandomBytes(_passwordSaltByteSize);
            var authKey = EncryptionHelper.GenerateKey(password, authKeySalt, _passwordByteSize, _passwordIterationCount);

            var result = EncryptionHelper.MergeArrays(_signatureByteSize, authKeySalt, keySalt, iv, cipherText);

            using (var hmac = new HMACSHA256(authKey))
            {
                var payloadToSignLength = result.Length - _signatureByteSize;
                var signatureTag = hmac.ComputeHash(result, 0, payloadToSignLength);
                signatureTag.CopyTo(result, payloadToSignLength);
            }

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encryptedValue, SecureString password)
        {
            var encrypted = Convert.FromBase64String(encryptedValue);

            if (encrypted is null || encrypted.Length < _minimumEncryptedMessageByteSize)
            {
                throw new ArgumentException("Invalid length for encrypted value.", nameof(encryptedValue));
            }

            var authKeySalt = encrypted.AsSpan(0, _passwordSaltByteSize).ToArray();
            var keySalt = encrypted.AsSpan(_passwordSaltByteSize, _passwordSaltByteSize).ToArray();
            var iv = encrypted.AsSpan(2 * _passwordSaltByteSize, _aesBlockByteSize).ToArray();
            var signatureTag = encrypted.AsSpan(encrypted.Length - _signatureByteSize, _signatureByteSize).ToArray();

            var cipherTextIndex = authKeySalt.Length + keySalt.Length + iv.Length;
            var cipherTextLength = encrypted.Length - cipherTextIndex - signatureTag.Length;

            var authKey = EncryptionHelper.GenerateKey(password, authKeySalt, _passwordByteSize, _passwordIterationCount);
            var key = EncryptionHelper.GenerateKey(password, keySalt, _passwordByteSize, _passwordIterationCount);

            // Verify
            using (var hmac = new HMACSHA256(authKey))
            {
                var payloadToSignLength = encrypted.Length - _signatureByteSize;
                var signatureTagExpected = hmac.ComputeHash(encrypted, 0, payloadToSignLength);

                // constant time checking to prevent timing attacks
                var signatureVerificationResult = 0;
                for (int i = 0; i < signatureTag.Length; i++)
                {
                    signatureVerificationResult |= signatureTag[i] ^ signatureTagExpected[i];
                }

                if (signatureVerificationResult != 0)
                {
                    throw new CryptographicException("Invalid Signature.");
                }
            }

            // Decrypt
            using var aes = EncryptionHelper.CreateAes();
            using var decryptor = aes.CreateDecryptor(key, iv);
            var plainText = decryptor.TransformFinalBlock(encrypted, cipherTextIndex, cipherTextLength);
            return _stringEncoding.GetString(plainText);
        }
    }
}
