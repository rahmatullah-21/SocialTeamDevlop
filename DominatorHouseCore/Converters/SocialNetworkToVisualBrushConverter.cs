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
                    case SocialNetworks.Facebook:
                        return "/DominatorUIUtility;component/Images/FacebookNew.png";
                    case SocialNetworks.Instagram:
                        return "/DominatorUIUtility;component/Images/InstagramNew.png";
                    case SocialNetworks.Twitter:
                        return "/DominatorUIUtility;component/Images/TwitterNew.png";
                    case SocialNetworks.Pinterest:
                        return "/DominatorUIUtility;component/Images/PinterestNew.png";
                    case SocialNetworks.LinkedIn:
                        return "/DominatorUIUtility;component/Images/TwitterNew.png";
                    case SocialNetworks.Reddit:
                        return "/DominatorUIUtility;component/Images/RedditNew.png";
                    case SocialNetworks.Quora:
                        return "/DominatorUIUtility;component/Images/QuoraNew.png";
                    //case SocialNetworks.Gplus:
                    //    return Application.Current?.FindResource("appbar_googleplus");
                    case SocialNetworks.Youtube:
                        return "/DominatorUIUtility;component/Images/YoutubeNew.png";
                    case SocialNetworks.Tumblr:
                        return "/DominatorUIUtility;component/Images/TumblrNew.png";
                    case SocialNetworks.Social:
                        return null;
                    case SocialNetworks.TikTok:
                        var icon = Application.Current?.FindResource("TikTok_Icon");
                        return icon;
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
