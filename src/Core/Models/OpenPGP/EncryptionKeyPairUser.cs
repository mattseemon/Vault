namespace Seemon.Vault.Core.Models.OpenPGP
{
    public class EncryptionKeyPairUser
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Comment { get; set; }

        public EncryptionKeyPairUser() { }

        public EncryptionKeyPairUser(string identity)
        {
            if (identity.Contains("<"))
                Email = identity.Substring(identity.IndexOf("<") + 1, identity.IndexOf(">") - identity.IndexOf("<") - 1);
            identity = identity.Replace($"<{Email}>", "").Trim();

            if (identity.Contains("("))
                Comment = identity.Substring(identity.IndexOf("(") + 1, identity.IndexOf(")") - identity.IndexOf("(") - 1);
            identity = identity.Replace($"({Comment})", "").Trim();

            Name = identity;
        }

        public override string ToString()
        {
            var identity = Name;
            if (!string.IsNullOrEmpty(Comment))
            {
                identity = $"{identity} ({Comment})";
            }
            if (!string.IsNullOrEmpty(Email))
            {
                identity = $"{identity} <{Email}>";
            }
            return identity;
        }
    }
}
