namespace Seemon.Vault.Helpers.Extensions
{
    public static class GeneralExtensions
    {
        public static bool IsNull<T>(this T self) => (self == null);

        public static bool IsNotNull<T>(this T self) => (self != null);
    }
}
