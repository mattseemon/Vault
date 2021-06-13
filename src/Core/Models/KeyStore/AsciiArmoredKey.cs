using LiteDB;
using System;

namespace Seemon.Vault.Core.Models.KeyStore
{
    public class AsciiArmoredKey
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonField("armored")]
        public string Armored { get; set; }

        [BsonField("created")]
        public DateTime Created { get; set; }

        [BsonField("modified")]
        public DateTime Modified { get; set; }
    }
}
