using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Seemon.Vault.Core.Contracts.Services;
using System.Windows;

namespace Seemon.Vault.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ILogger<ISettingsService> _logger;

        public SettingsService(ILogger<ISettingsService> logger) => _logger = logger;

        public T Get<T>(string key, T @default)
        {
            if (!Application.Current.Properties.Contains(key))
            {
                Application.Current.Properties[key] = @default;
                return @default;
            }

            return Get<T>(key);
        }

        public T Get<T>(string key)
        {
            if (!Application.Current.Properties.Contains(key))
            {
                _logger.LogInformation($"Could not find setting with name - {key}.");
                return default;
            }

            if (Application.Current.Properties[key] is JObject)
            {
                var jObject = Application.Current.Properties[key] as JObject;
                Application.Current.Properties[key] = jObject.ToObject<T>();
            }

            if (Application.Current.Properties[key] is JArray)
            {
                var jArray = Application.Current.Properties[key] as JArray;
                Application.Current.Properties[key] = jArray.ToObject<T>();
            }

            return (T)Application.Current.Properties[key];
        }

        public void Set<T>(string key, T value) => Application.Current.Properties[key] = value;
    }
}
