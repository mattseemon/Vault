using System;
using System.Numerics;

namespace Seemon.Vault.Core.Helpers.Services
{
    // Ported from JavaScript to C#
    // Credit: https://github.com/certsimple/ssl-rsa-strength
    public static class PGPHelper
    {
        public static int GetRSAStrength(int keySize)
        {
            var bigIntString = BigInteger.Pow(new BigInteger(2), keySize).ToString();
            var naturalLog = (bigIntString.Length * Math.Log(10)) + Math.Log(double.Parse($"0.{bigIntString}"));
            var strength = Math.Floor(Math.Log2(Math.Exp(Math.Cbrt(64 / 9 * naturalLog) * Math.Pow(Math.Log(naturalLog), decimal.ToDouble(2m / 3m)))));

            return Convert.ToInt32(strength);
        }
    }
}
