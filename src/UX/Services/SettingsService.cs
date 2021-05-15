using Newtonsoft.Json.Linq;
using Seemon.Vault.Core.Contracts.Services;

namespace Seemon.Vault.Services
{
    public class SettingsService : ISettingsService
    {
        public SettingsService() { }

        public T Get<T>(string key, T @default)
        {
            if (!App.Current.Properties.Contains(key))
            {
                App.Current.Properties[key] = @default;
                return @default;
            }

            return Get<T>(key);
        }

        public T Get<T>(string key)
        {
            if (!App.Current.Properties.Contains(key))
            {
                throw new System.Exception($"Could not find setting with identifier - {key}.");
            }

            if (App.Current.Properties[key] is JObject)
            {
                JObject jObject = App.Current.Properties[key] as JObject;
                App.Current.Properties[key] = jObject.ToObject<T>();
            }

            if (App.Current.Properties[key] is JArray)
            {
                JArray jArray = App.Current.Properties[key] as JArray;
                App.Current.Properties[key] = jArray.ToObject<T>();
            }

            return (T)App.Current.Properties[key];
        }

        public void Set<T>(string key, T value) => App.Current.Properties[key] = value;
    }
}
