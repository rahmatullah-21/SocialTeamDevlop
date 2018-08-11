using System;
using System.Globalization;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.ToString().Equals("Active") ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var valu = (bool)value;
                return valu ? "Active" : "Paused";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return value;
            }
        }
    }
}