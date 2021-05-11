using Newtonsoft.Json.Linq;
using Seemon.Vault.Core.Contracts.Services;

namespace Seemon.Vault.Services
{
    public class SettingsService : ISettingsService
    {
        public SettingsService()
        {
        }

        public T Get<T>(string key, T @default)
        {
            if (!App.Current.Properties.Contains(key))
            {
                return @default;
            }

            if (App.Current.Properties[key] is JObject)
            {
                JObject jObject = App.Current.Properties[key] as JObject;
                App.Current.Properties[key] = jObject.ToObject<T>();
            }

            return (T)App.Current.Properties[key];
        }

        public void Set<T>(string key, T value)
        {
            App.Current.Properties[key] = value;
        }
    }
}
