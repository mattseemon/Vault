using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Seemon.Vault.Core.Helpers.Extensions
{
    public static class SecureStringExtensions
    {
        [SuppressUnmanagedCodeSecurity]
        public static string ToUnsecuredString(this SecureString secureString)
        {
            IntPtr bstrPtr = IntPtr.Zero;

            if (secureString is null)
            {
                return string.Empty;
            }
            if (secureString.Length > 0)
            {
                try
                {
                    bstrPtr = Marshal.SecureStringToBSTR(secureString);
                    return Marshal.PtrToStringBSTR(bstrPtr);
                }
                finally
                {
                    if (bstrPtr != IntPtr.Zero)
                    {
                        Marshal.ZeroFreeBSTR(bstrPtr);
                    }
                }
            }

            return string.Empty;
        }

        public static void CopyInto(this SecureString source, SecureString destination)
        {
            destination.Clear();
            foreach (var c in source.ToUnsecuredString())
            {
                destination.AppendChar(c);
            }
        }

        public static SecureString ToSecureString(this string plainString)
        {
            var secureString = new SecureString();
            if (!string.IsNullOrEmpty(plainString))
            {
                foreach (var c in plainString)
                {
                    secureString.AppendChar(c);
                }
            }
            return secureString;
        }

        public static bool Equals(this SecureString source, SecureString destination)
        {
            if (source.Length != destination.Length)
            {
                return false;
            }

            var strSource = source.ToUnsecuredString();
            var strDest = destination.ToUnsecuredString();

            return strSource.Equals(strDest, StringComparison.Ordinal);
        }

        public static string GetHashString(this SecureString source)
        {
            using var hash = SHA512.Create();
            var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(source.ToUnsecuredString()));

            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
