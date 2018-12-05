using System;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    [ValueConversion(typeof(string), typeof(string))]

    public class StringToPasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "*****";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}