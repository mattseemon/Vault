using LiteDB;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Seemon.Vault.Core.Helpers.Extensions;
using System;

namespace Seemon.Vault.Core.Models.KeyStore
{
    public class KeyPairInfo : ObservableObject
    {
        private Guid _id;
        private long _keyId;
        private string _name;
        private string _email;
        private string _comment;
        private int _keySize;
        private DateTime _created;
        private DateTime? _expiry;

        private bool _hasPrivateKeys = true;
        private bool _isDefault = false;

        [BsonId()]
        public Guid Id
        {
            get => _id; set => SetProperty(ref _id, value);
        }

        [BsonField("keyId")]
        public long KeyId
        {
            get => _keyId; set => SetProperty(ref _keyId, value);
        }

        [BsonIgnore]
        public string KeyIDHex => KeyId.GetKeyIdHex();

        [BsonField("keySize")]
        public int KeySize
        {
            get => _keySize; set => SetProperty(ref _keySize, value);
        }

        [BsonField("name")]
        public string Name
        {
            get => _name; set => SetProperty(ref _name, value);
        }

        [BsonField("email")]
        public string Email
        {
            get => _email; set => SetProperty(ref _email, value);
        }

        [BsonField("comment")]
        public string Comment
        {
            get => _comment; set => SetProperty(ref _comment, value);
        }

        [BsonField("created")]
        public DateTime Created
        {
            get => _created; set => SetProperty(ref _created, value);
        }

        [BsonField("expiry")]
        public DateTime? Expiry
        {
            get => _expiry; set => SetProperty(ref _expiry, value);
        }

        [BsonIgnore]
        public bool IsExpired => Expiry.HasValue && DateTime.Now > Expiry.Value;

        [BsonField("hasPrivateKeys")]
        public bool HasPrivateKeys
        {
            get => _hasPrivateKeys; set => SetProperty(ref _hasPrivateKeys, value);
        }

        [BsonField("isDefault")]
        public bool IsDefault
        {
            get => _isDefault; set => SetProperty(ref _isDefault, value);
        }

        [BsonIgnore]
        public bool IsSelected { get; set; }

        [BsonField("modified")]
        public DateTime Modified { get; set; }

        public void SetIdentity(string identity)
        {
            if (identity.Contains("<"))
            {
                Email = identity.Substring(identity.IndexOf("<") + 1, identity.IndexOf(">") - identity.IndexOf("<") - 1);
            }
            identity = identity.Replace($"<{Email}>", "").Trim();

            if (identity.Contains("("))
            {
                Comment = identity.Substring(identity.IndexOf("(") + 1, identity.IndexOf(")") - identity.IndexOf("(") - 1);
            }
            identity = identity.Replace($"({Comment})", "").Trim();

            Name = identity;
        }
    }
}
