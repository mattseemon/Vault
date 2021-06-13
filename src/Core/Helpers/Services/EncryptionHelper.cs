using Seemon.Vault.Core.Helpers.Extensions;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Seemon.Vault.Core.Helpers.Services
{
    public static class EncryptionHelper
    {
        private static readonly Encoding _stringEncoding = Encoding.UTF8;
        private static readonly ThreadLocal<RandomNumberGenerator> _random = new(() => new RNGCryptoServiceProvider());

        public static byte[] GenerateKey(SecureString password, byte[] passwordSalt, int passwordByteSize, int iterations = 100000)
        {
            var keyBytes = _stringEncoding.GetBytes(password.ToUnsecuredString());

            using var derivator = new Rfc2898DeriveBytes(keyBytes, passwordSalt, iterations, HashAlgorithmName.SHA256);
            return derivator.GetBytes(passwordByteSize);
        }

        public static byte[] GenerateRandomBytes(int numberOfBytes)
        {
            var randomBytes = new byte[numberOfBytes];
            _random.Value.GetBytes(randomBytes);
            return randomBytes;
        }

        public static byte[] MergeArrays(int additionalCapacity = 0, params byte[][] arrays)
        {
            var merged = new byte[arrays.Sum(a => a.Length) + additionalCapacity];
            var mergeIndex = 0;
            for (var i = 0; i < arrays.GetLength(0); i++)
            {
                arrays[i].CopyTo(merged, mergeIndex);
                mergeIndex += arrays[i].Length;
            }
            return merged;
        }

        public static Aes CreateAes()
        {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }
    }
}
