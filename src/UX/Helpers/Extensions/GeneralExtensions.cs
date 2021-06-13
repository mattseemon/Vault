using Newtonsoft.Json;

namespace Seemon.Vault.Helpers.Extensions
{
    public static class GeneralExtensions
    {
        public static T DeepCopy<T>(this T self)
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static bool IsNull<T>(this T self) => self is null;

        public static bool IsNotNull<T>(this T self) => self is not null;

        public static T GetPropertyValue<T>(this object self, string property) 
            => (T)self.GetType().GetProperty(property).GetValue(self, null);
    }
}
