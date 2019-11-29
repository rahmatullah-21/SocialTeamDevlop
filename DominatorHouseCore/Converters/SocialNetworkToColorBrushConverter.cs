using DominatorHouseCore.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DominatorHouseCore.Converters
{
    public class SocialNetworkToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var network = value as SocialNetworks?;
            if (network.HasValue)
            {
                switch (network.Value)
                {
                    case SocialNetworks.Instagram:
                        return Brushes.Red;
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
