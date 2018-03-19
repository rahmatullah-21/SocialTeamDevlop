using System;
using System.Globalization;
using System.Windows.Data;
using DominatorHouse.UsefullUtilitiesLibrary;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(int), typeof(DateTime))]

    public class EpochToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return DateTimeHelper.EpochToDateTimeUtc(int.Parse(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}