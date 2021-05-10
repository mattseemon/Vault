using System;

namespace Seemon.Vault.Core.Models
{
    public class ApplicationTheme
    {
        public enum ThemeBase
        {
            System,
            Dark,
            Light
        }

        public static ApplicationTheme Default = new ApplicationTheme("System.System");

        public ApplicationTheme(string themeInfo)
        {
            var infos = themeInfo.Split(".");
            if (infos.Length != 2)
            {
                throw new ArgumentNullException("theme", "Invalid theme data.");
            }
            Enum.TryParse(infos[0], out ThemeBase themeBase);

            Base = themeBase;
            Accent = infos[1];
        }

        public ThemeBase Base { get; set; }

        public string Accent { get; set; }

        public override string ToString() => $"{Base}.{Accent}";

        public override bool Equals(object obj) => string.Equals(ToString(), obj.ToString());

        public override int GetHashCode() => ToString().GetHashCode();
    }
}
