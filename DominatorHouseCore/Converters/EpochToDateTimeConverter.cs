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
                string interval = string.Empty;

                if (value.ToString().Length > 13)
                    interval = value.ToString().Substring(0, 13);
                else
                    interval = value.ToString();

                if (value.ToString().Length == 10)
                    return DateTimeUtilities.EpochToDateTimeLocal(int.Parse(interval.ToString()));
                else if (value.ToString().Length > 10)
                    return DateTimeUtilities.EpochToDateTimeLocal(Int64.Parse(interval.ToString()));
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