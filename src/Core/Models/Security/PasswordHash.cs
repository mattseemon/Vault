namespace Seemon.Vault.Core.Models.Security
{
    public class PasswordHash
    {
        public string Salt { get; set; }

        public string Hash { get; set; }
    }
}
