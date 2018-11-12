using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class IntToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "0" ? "Not Updated Yet" : System.Convert.ToInt32(value).EpochToDateTimeLocal().ToString("dd MMM yyyy HH:mm:ss tt");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
