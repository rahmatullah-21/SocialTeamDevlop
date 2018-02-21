using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class ListCountBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && ((IEnumerable)value).Cast<object>().ToList().Count > 1 ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}