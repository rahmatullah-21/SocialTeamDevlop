using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Converters
{
    [ValueConversion(typeof(SocialNetworks), typeof(Visibility))]
    public class FeatureVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            var network = (SocialNetworks)value;
            if (string.IsNullOrEmpty(network.ToString()))
                return Visibility.Collapsed;

            var visibilityStatus = FeatureFlags.Check(network.ToString()) ? Visibility.Visible : Visibility.Collapsed;

            return visibilityStatus;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Collapsed;
        }
    }
}
