using System;

namespace Seemon.Vault.Core.Exceptions
{
    public class VaultException : Exception
    {
        public VaultException(string message) : base(message) { }

        public VaultException(string message, Exception innerException) : base(message, innerException) { }
    }
}
