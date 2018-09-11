using System;
using System.Globalization;
using System.Windows.Data;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(DateTime))]

    public class EpochToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value.ToString().Length == 10)
                    return DateTimeUtilities.EpochToDateTimeLocal(int.Parse(value.ToString()));
                else if (value.ToString().Length > 10)
                    return DateTimeUtilities.EpochToDateTimeUtc(Int64.Parse(value.ToString()));
                return value;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}