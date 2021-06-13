using Seemon.Vault.Core.Helpers.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Seemon.Vault.Helpers.Converters
{
    public class LongToHexConverter : MarkupExtension, IValueConverter
    {
        public bool ShortForm { get; set; }

        public LongToHexConverter() => ShortForm = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is not long ? string.Empty : ((long)value).GetKeyIdHex(ShortForm);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
