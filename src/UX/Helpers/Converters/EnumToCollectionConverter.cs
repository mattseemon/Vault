using Seemon.Vault.Helpers.Extensions;
using Seemon.Vault.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Seemon.Vault.Helpers.Converters
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => GetAllValuesAndDescriptions(value.GetType());

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        private IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type t)
        {
            return !t.IsEnum
                ? throw new ArgumentException($"{nameof(t)} must be an enum type")
                : Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.GetDescription() }).ToList();
        }
    }
}
