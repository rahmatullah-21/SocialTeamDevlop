using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        public bool IsInversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var visibility = !string.IsNullOrEmpty(value?.ToString()) ? Visibility.Visible : Visibility.Collapsed;

            if (IsInversed)
            {
                visibility = string.IsNullOrEmpty(value?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
            }

            return visibility;
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