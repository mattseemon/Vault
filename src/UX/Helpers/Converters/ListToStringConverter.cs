using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Seemon.Vault.Helpers.Converters
{
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ListToStringConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => targetType != typeof(string)
                ? throw new InvalidOperationException("The target must be string.")
                : string.Join(Environment.NewLine, ((List<string>)value).ToArray());

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
