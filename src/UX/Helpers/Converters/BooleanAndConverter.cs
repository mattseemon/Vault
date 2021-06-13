using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Seemon.Vault.Helpers.Converters
{
    public class BooleanAndConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if ((value is bool boolean) && !boolean)
                {
                    return false;
                }
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) 
            => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
