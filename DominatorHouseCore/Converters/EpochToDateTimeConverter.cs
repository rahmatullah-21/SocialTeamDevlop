using System;
using System.Globalization;
using System.Windows.Data;
using DominatorHouseCore.Utility;
using Socinator.UsefullUtilitiesLibrary;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(DateTime))]

    public class EpochToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return DateTimeUtilities.EpochToDateTimeUtc(int.Parse(value.ToString()));
            }
            catch (Exception)
            {
                return null;
            }
        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}