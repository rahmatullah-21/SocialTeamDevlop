using System;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class StringPercentageTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return value + " %";
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}