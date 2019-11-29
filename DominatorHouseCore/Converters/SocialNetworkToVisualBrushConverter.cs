using DominatorHouseCore.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DominatorHouseCore.Converters
{
    public class SocialNetworkToVisualBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var network = value as SocialNetworks?;
            if (network.HasValue)
            {
                switch (network.Value)
                {
                    case SocialNetworks.Instagram:
                        return Application.Current?.FindResource("Instagram");                  
                    case SocialNetworks.Social:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return null;
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
