using System;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class BoolToValueConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        public object NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            if (!boolValue.HasValue)
            {
                return NullValue;
            }

            return boolValue.Value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
