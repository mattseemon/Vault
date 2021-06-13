using LiteDB;
using LiteDB.Engine;
using Microsoft.Extensions.Logging;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Exceptions;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Helpers.Extensions;
using Seemon.Vault.Core.Models.KeyStore;
using Seemon.Vault.Core.Models.Security;
using Seemon.Vault.Core.Models.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace Seemon.Vault.Core.Services
{
    public class KeyStoreService : IKeyStoreService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IPasswordService _passwordService;
        private readonly ISettingsService _settingsService;
        private readonly ILogger<IKeyStoreService> _logger;
        private readonly IPasswordCacheService _passwordCacheService;
        private const string COLLECTION_STORE_CREDENTIALS = "COLL_STORE_CREDENTIALS";
        private const string COLLECTION_STORE_KEY_PAIRS = "COLL_STORE_KEY_PAIRS";
        private const string COLLECTION_STORE_ASCII_ARMORED = "COLL_STORE_ASCII_ARMORED";
        private string _storePath;

        public KeyStoreService(IEncryptionService encryptionService, IPasswordService passwordService, ISettingsService settingsService,
            ILogger<IKeyStoreService> logger, IPasswordCacheService passwordCacheService)
        {
            _encryptionService = encryptionService;
            _passwordService = passwordService;
            _settingsService = settingsService;
            _logger = logger;
            _passwordCacheService = passwordCacheService;
        }

        public bool InitializeKeyStore(string path)
        {
            try
            {
                _storePath = path;
                if (!File.Exists(_storePath))
                {
                    var password = _passwordService.CreatePassword(Constants.PASSWORD_CACHE_KEY_STORE);
                    return password is not null && CreateKeyStore(_storePath, password);
                }
                else
                {
                    var settings = _settingsService.Get<SecuritySettings>(Constants.SETTINGS_SECURITY);
                    var maxAttempts = settings.SelfErase ? settings.SelfEraseOnFailureCount : 0;
                    var currentAttempt = settings.SelfErase ? 1 : 0;

                    var password = _passwordService.GetPassword(Constants.PASSWORD_CACHE_KEY_STORE, currentAttempt++, true);
                    if (password is null)
                    {
                        return false;
                    }

                    bool verified = false;
                    while (!verified)
                    {
                        verified = VerifyPassword(password);
                        if(!verified && currentAttempt > maxAttempts)
                        {
                            // TODO Implement Destroy Data
                            return false;
                        }
                        if (!verified)
                        {
                            password = _passwordService.GetPassword(Constants.PASSWORD_CACHE_KEY_STORE, currentAttempt++, true);
                        }
                        if(password is null)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to KeyStore");
                return false;
            }
        }

        public bool ChangePassword()
        {
            var exclusionList = GetExclusionList();
            var response = _passwordService.ChangePassword(Constants.PASSWORD_CACHE_KEY_STORE, exclusionList);

            if (response is not null)
            {
                using var db = new LiteDatabase(BuildConnectionString(_storePath, response.OldPassword));
                db.Rebuild(new RebuildOptions
                {
                    Collation = Collation.Default,
                    Password = response.NewPassword.ToUnsecuredString()
                });

                var storeCredentials = db.GetCollection<StoreCredentials>(COLLECTION_STORE_CREDENTIALS);
                foreach (var item in storeCredentials.Find(x => x.IsActive == true))
                {
                    item.IsActive = false;
                    storeCredentials.Update(item);
                }
                storeCredentials.Insert(new StoreCredentials
                {
                    Password = _passwordService.HashPassword(response.NewPassword)
                });

                var collection = db.GetCollection<AsciiArmoredKey>(COLLECTION_STORE_ASCII_ARMORED);
                
                foreach(var item in collection.FindAll())
                {
                    item.Armored = _encryptionService.Encrypt(_encryptionService.Decrypt(item.Armored, response.OldPassword), response.NewPassword);
                    collection.Update(item);
                }

                return true;
            }
            return false;
        }

        public async Task<bool> ChangePasswordAsync()
        {
            return await Task.Run(() => ChangePassword());
        }

        public bool VerifyPassword(SecureString password)
        {
            try
            {
                using var db = new LiteDatabase(BuildConnectionString(_storePath, password, true));
                var collection = db.GetCollection<StoreCredentials>(COLLECTION_STORE_CREDENTIALS);
                var credentials = collection.Find(x => x.IsActive).Last();

                return _passwordService.VerifyPassword(password, credentials.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not connect to KeyStore");
                return false;
            }
        }

        private bool CreateKeyStore(string path, SecureString password)
        {
            try
            {
                using var db = new LiteDatabase(BuildConnectionString(path, password));
                var credentialsCollection = db.GetCollection<StoreCredentials>(COLLECTION_STORE_CREDENTIALS);
                credentialsCollection.Insert(new StoreCredentials
                {
                    Password = _passwordService.HashPassword(password)
                });
                return true;
            }
            catch { return false; }
        }

        private static ConnectionString BuildConnectionString(string path, SecureString password, bool readOnly = false)
        {
            return new ConnectionString
            {
                Filename = path,
                Collation = Collation.Default,
                Password = password.ToUnsecuredString(),
                Upgrade = true,
                ReadOnly = readOnly
            };
        }

        public void InsertKeyPair(Guid id, KeyPairInfo keyPair, SecureString asciiArmored)
        {
            var password = _passwordService.GetPassword(Constants.PASSWORD_CACHE_KEY_STORE);

            AsciiArmoredKey armoredKey = new AsciiArmoredKey
            {
                Id = keyPair.Id,
                Armored = _encryptionService.Encrypt(asciiArmored.ToUnsecuredString(), password),
                Created = keyPair.Created,
                Modified = keyPair.Modified
            };

            try
            {
                using var db = new LiteDatabase(BuildConnectionString(_storePath, password));
                var keyPairCollection = db.GetCollection<KeyPairInfo>(COLLECTION_STORE_KEY_PAIRS);
                keyPairCollection.Insert(keyPair);

                var asciiArmoredCollection = db.GetCollection<AsciiArmoredKey>(COLLECTION_STORE_ASCII_ARMORED);
                asciiArmoredCollection.Insert(armoredKey);
            }
            catch { }
        }

        public void UpdateKeyPair(Guid id, KeyPairInfo keyPair, SecureString asciiArmored)
        {
            var password = _passwordService.GetPassword(Constants.PASSWORD_CACHE_KEY_STORE);

            try
            {
                using var db = new LiteDatabase(BuildConnectionString(_storePath, password));
                var keyPairCollection = db.GetCollection<KeyPairInfo>(COLLECTION_STORE_KEY_PAIRS);
                var asciiArmoredCollection = db.GetCollection<AsciiArmoredKey>(COLLECTION_STORE_ASCII_ARMORED);

                var dateModified = DateTime.Now;
                keyPair.Modified = dateModified;

                keyPairCollection.Update(keyPair);

                var armoredKey = asciiArmoredCollection.Find(x => x.Id == id).FirstOrDefault();
                armoredKey.Armored = _encryptionService.Encrypt(asciiArmored.ToUnsecuredString(), password);
                armoredKey.Modified = dateModified;

                asciiArmoredCollection.Update(armoredKey);
            }
            catch { }
        }

        public List<KeyPairInfo> GetKeyPairs()
        {
            var password = _passwordService.GetPassword(Constants.PASSWORD_CACHE_KEY_STORE);

            var keyPairs = new List<KeyPairInfo>();
            try
            {
                using var db = new LiteDatabase(BuildConnectionString(_storePath, password));
                var keyPairCollection = db.GetCollection<KeyPairInfo>(COLLECTION_STORE_KEY_PAIRS);

                keyPairs = keyPairCollection.FindAll().ToList();
            }
            catch { }
            return keyPairs;
        }

        public async Task<List<KeyPairInfo>> GetKeyPairsAsync()
        {
            return await Task.Run(() => GetKeyPairs());
        }

        private List<PasswordHash> GetExclusionList()
        {
            var password = _passwordService.GetPassword(Constants.PASSWORD_CACHE_KEY_STORE);
            var securitySettings = _settingsService.Get<SecuritySettings>(Constants.SETTINGS_SECURITY);

            using var db = new LiteDatabase(BuildConnectionString(_storePath, password));
            var collection = db.GetCollection<StoreCredentials>(COLLECTION_STORE_CREDENTIALS);
            return collection.FindAll().TakeLast(securitySettings.MinimumUniquePasswordCount).Select(x => x.Password).ToList();
        }

        public void DeleteKeyPair(Guid id)
        {
            var password = _passwordService.GetPassword(Constants.PASSWORD_CACHE_KEY_STORE);
            using var db = new LiteDatabase(BuildConnectionString(_storePath, password));
            var collKeyPairs = db.GetCollection<KeyPairInfo>(COLLECTION_STORE_KEY_PAIRS);
            collKeyPairs.Delete(id);

            var collAscii = db.GetCollection<AsciiArmoredKey>(COLLECTION_STORE_ASCII_ARMORED);
            collAscii.Delete(id);
        }

        public void DeleteKeyPairs(List<Guid> ids)
        {
            var password = _passwordService.GetPassword(Constants.PASSWORD_CACHE_KEY_STORE);
            using var db = new LiteDatabase(BuildConnectionString(_storePath, password));

            var collKeyPairs = db.GetCollection<KeyPairInfo>(COLLECTION_STORE_KEY_PAIRS);
            var collAscii = db.GetCollection<AsciiArmoredKey>(COLLECTION_STORE_ASCII_ARMORED);

            foreach (var id in ids)
            {
                collKeyPairs.Delete(id);
                collAscii.Delete(id);
            }
        }

        public bool SetDefaultKeyPair(Guid id)
        {
            try
            {
                var password = _passwordCacheService.Get(Constants.PASSWORD_CACHE_KEY_STORE);
                using var db = new LiteDatabase(BuildConnectionString(_storePath, password));

                var collection = db.GetCollection<KeyPairInfo>(COLLECTION_STORE_KEY_PAIRS);
                foreach (var keyPair in collection.FindAll())
                {
                    keyPair.IsDefault = (keyPair.Id == id);
                    collection.Update(keyPair);
                }
                return true;
            }
            catch { return false; }
        }

        public SecureString GetKeyPairAscii(Guid id)
        {
            try
            {
                var password = _passwordCacheService.Get(Constants.PASSWORD_CACHE_KEY_STORE);
                using var db = new LiteDatabase(BuildConnectionString(_storePath, password));

                var collection = db.GetCollection<AsciiArmoredKey>(COLLECTION_STORE_ASCII_ARMORED);
                var asciiArmoredKey = collection.Find(x => x.Id == id).FirstOrDefault();

                return asciiArmoredKey is not null ? _encryptionService.Decrypt(asciiArmoredKey.Armored, password).ToSecureString() : null;
            }
            catch { throw new VaultException($"Error finding key with id: \"{id}\""); }
        }

        public KeyPairInfo GetKeyPair(Guid id)
        {
            try
            {
                var password = _passwordCacheService.Get(Constants.PASSWORD_CACHE_KEY_STORE);
                using var db = new LiteDatabase(BuildConnectionString(_storePath, password));

                var collection = db.GetCollection<KeyPairInfo>(COLLECTION_STORE_KEY_PAIRS);
                var keyPairInfo = collection.Find(x => x.Id == id).FirstOrDefault();

                return keyPairInfo is not null ? keyPairInfo : null;
            }
            catch { throw new VaultException($"Error finding key with id: \"{id}\""); }
        }
    }
}
