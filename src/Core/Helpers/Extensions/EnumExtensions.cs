using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Seemon.Vault.Core.Helpers.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            try
            {
                var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Any())
                {
                    return (attributes.First() as DescriptionAttribute).Description;
                }

                var textInfo = CultureInfo.CurrentCulture.TextInfo;
                return textInfo.ToTitleCase(textInfo.ToLower(value.ToString()));
            }
            catch { return string.Empty; }
        }
    }
}
