using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DominatorHouseCore.Converters
{
    public class SocialNetworkToLinkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var network = value as SocialNetworks?;
            if (network.HasValue)
            {
                switch (network)
                {
                    case SocialNetworks.Social:
                        return ConstantVariable.SocialAccountManagerVideoLink;
                    case SocialNetworks.Instagram:
                        return ConstantVariable.IgAccountManagerVideoLink;
                   
                    default:
                        return ConstantVariable.SocialAccountManagerVideoLink;
                }
            }

            return ConstantVariable.SocialAccountManagerVideoLink;
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
