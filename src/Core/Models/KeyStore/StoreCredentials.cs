using LiteDB;
using Seemon.Vault.Core.Models.Security;
using System;

namespace Seemon.Vault.Core.Models.KeyStore
{
    public class StoreCredentials
    {
        [BsonId(true)]
        public ObjectId Id { get; set; }

        public PasswordHash Password { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;
    }
}
