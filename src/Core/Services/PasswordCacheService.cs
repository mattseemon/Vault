using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Seemon.Vault.Core.Contracts.Services;
using Seemon.Vault.Core.Helpers;
using Seemon.Vault.Core.Models.Settings;
using System;
using System.Security;

namespace Seemon.Vault.Core.Services
{
    public class PasswordCacheService : IPasswordCacheService
    {
        private readonly ILogger<IPasswordCacheService> _logger;
        private readonly ISettingsService _settingsService;

        private readonly MemoryCache _cache;

        public PasswordCacheService(ILogger<IPasswordCacheService> logger, ISettingsService settingsService)
        {
            _logger = logger;
            _settingsService = settingsService;

            _logger.LogInformation("Initialize password cache");
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 100
            });
        }

        public void Clear(object key) => _cache.Remove(key);

        public SecureString Get(object key) => _cache.TryGetValue(key, out SecureString password) ? password : null;

        public void Set(object key, SecureString password, bool expires = false)
        {
            var pgpSettings = _settingsService.Get<PGPSettings>(Constants.SETTINGS_PGP);

            if (key.ToString() == Constants.PASSWORD_CACHE_KEY_STORE || pgpSettings.CachePassword)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSize(1);

                if (expires && pgpSettings.AutoClear)
                {
                    cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(pgpSettings.AutoClearDuration));
                }

                _cache.Set(key, password, cacheEntryOptions); 
            }
        }
    }
}
