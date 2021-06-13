using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Seemon.Vault.Helpers.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class IntToVisbilityConverter : MarkupExtension, IValueConverter
    {
        public int MinimumThreshold { get; set; }

        public IntToVisbilityConverter() => MinimumThreshold = 1;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int)
                return Visibility.Collapsed;

            var objValue = (int)value;
            return objValue <= MinimumThreshold ? Visibility.Collapsed : (object)Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
