using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Behaviours
{
    public class NetworkAvailablity
    {       
        #region Facebook

        public static readonly DependencyProperty FbElementVisibleProperty = DependencyProperty.RegisterAttached(
            "FbElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Facebook)));

        public static Visibility GetFbElementVisible(DependencyObject element)
        {
            return (Visibility) element.GetValue(FbElementVisibleProperty);
        }


        #endregion

        #region Instagram

        public static readonly DependencyProperty IgElementVisibleProperty = DependencyProperty.RegisterAttached(
            "IgElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Instagram)));
        
        #endregion

        #region Twitter


        public static readonly DependencyProperty TwtElementVisibleProperty = DependencyProperty.RegisterAttached(
            "TwtElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Twitter)));

        #endregion

        #region LinkedIn

        public static readonly DependencyProperty LdElementVisibleProperty = DependencyProperty.RegisterAttached(
            "LdElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.LinkedIn)));

        #endregion

        #region Tumblr

        public static readonly DependencyProperty TumblrElementVisibleProperty = DependencyProperty.RegisterAttached(
            "TumblrElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Tumblr)));

        #endregion

        #region Pinterest

        public static readonly DependencyProperty PdElementVisibleProperty = DependencyProperty.RegisterAttached(
            "PdElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Pinterest)));

        #endregion
    }
}