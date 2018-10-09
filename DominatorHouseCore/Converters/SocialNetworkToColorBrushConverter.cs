using DominatorHouseCore.Enums;
using System;
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
                    case SocialNetworks.Facebook:
                        return (SolidColorBrush)(new BrushConverter().ConvertFrom("#003569"));
                    case SocialNetworks.Instagram:
                        return Brushes.Red;
                    case SocialNetworks.Twitter:
                        return (SolidColorBrush)(new BrushConverter().ConvertFrom("#3897F0"));
                    case SocialNetworks.Pinterest:
                        return Brushes.Red;
                    case SocialNetworks.LinkedIn:
                        return Brushes.DodgerBlue;
                    case SocialNetworks.Reddit:
                        return Brushes.OrangeRed;
                    case SocialNetworks.Quora:
                        return (SolidColorBrush)(new BrushConverter().ConvertFrom("#28A745"));
                    case SocialNetworks.Gplus:
                        return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
                    case SocialNetworks.Youtube:
                        return (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
                    case SocialNetworks.Tumblr:
                        return Brushes.DimGray;
                    case SocialNetworks.Social:
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
