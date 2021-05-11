using Seemon.Vault.Helpers.Extensions;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Seemon.Vault.Helpers.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class CaseConverter : MarkupExtension, IValueConverter
    {
        public CharacterCasing Case { get; set; }

        public CaseConverter() => this.Case = CharacterCasing.Upper;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if (str.IsNotNull())
            {
                return this.Case switch
                {
                    CharacterCasing.Lower => str.ToLower(),
                    CharacterCasing.Normal => str,
                    CharacterCasing.Upper => str.ToUpper(),
                    _ => str,
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
