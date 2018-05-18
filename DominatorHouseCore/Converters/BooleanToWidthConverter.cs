using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
  
    public class BooleanToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,object parameter, CultureInfo culture)
        {
            return ((bool)value) ? 0 : int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return 0;
        }
    }
    public class ProxyPortToIpAddressConvertor : IMultiValueConverter

    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return String.Concat(values[0], ":", values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as string).Split(':');
        }
    }
}
