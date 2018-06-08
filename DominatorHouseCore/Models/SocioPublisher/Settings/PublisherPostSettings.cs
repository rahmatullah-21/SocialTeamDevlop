using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    public class PublisherPostSettings 
    {

        public static readonly DependencyProperty IsFacebookAvailableProperty = DependencyProperty.RegisterAttached(
            "IsFacebookAvailable", typeof(bool), typeof(PublisherPostSettings), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Facebook.ToString())));

        public static void SetIsFacebookAvailable(DependencyObject element, bool value)
        {
            element.SetValue(IsFacebookAvailableProperty, value);
        }

        public static bool GetIsFacebookAvailable(DependencyObject element)
        {
            return (bool) element.GetValue(IsFacebookAvailableProperty);
        }


        public static readonly DependencyProperty IsInstagramAvailableProperty = DependencyProperty.RegisterAttached(
            "IsInstagramAvailable", typeof(bool), typeof(PublisherPostSettings), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Instagram.ToString())));

        public static void SetIsInstagramAvailable(DependencyObject element, bool value)
        {
            element.SetValue(IsInstagramAvailableProperty, value);
        }

        public static bool GetIsInstagramAvailable(DependencyObject element)
        {
            return (bool) element.GetValue(IsInstagramAvailableProperty);
        }


        public static readonly DependencyProperty IsTwitterAvailableProperty = DependencyProperty.RegisterAttached(
            "IsTwitterAvailable", typeof(bool), typeof(PublisherPostSettings), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Twitter.ToString())));

        public static void SetIsTwitterAvailable(DependencyObject element, bool value)
        {
            element.SetValue(IsTwitterAvailableProperty, value);
        }

        public static bool GetIsTwitterAvailable(DependencyObject element)
        {
            return (bool) element.GetValue(IsTwitterAvailableProperty);
        }


        public static readonly DependencyProperty IsLinkedInAvailableProperty = DependencyProperty.RegisterAttached(
            "IsLinkedInAvailable", typeof(bool), typeof(PublisherPostSettings), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Tumblr.ToString())));

        public static void SetIsLinkedInAvailable(DependencyObject element, bool value)
        {
            element.SetValue(IsLinkedInAvailableProperty, value);
        }

        public static bool GetIsLinkedInAvailable(DependencyObject element)
        {
            return (bool) element.GetValue(IsLinkedInAvailableProperty);
        }

        public static readonly DependencyProperty IsTumblrAvailableProperty = DependencyProperty.RegisterAttached(
            "IsTumblrAvailable", typeof(bool), typeof(PublisherPostSettings), new PropertyMetadata(default(bool)));

        public static void SetIsTumblrAvailable(DependencyObject element, bool value)
        {
            element.SetValue(IsTumblrAvailableProperty, value);
        }

        public static bool GetIsTumblrAvailable(DependencyObject element)
        {
            return (bool) element.GetValue(IsTumblrAvailableProperty);
        }

        public static readonly DependencyProperty FdPostSettingsProperty = DependencyProperty.RegisterAttached(
            "FdPostSettings", typeof(FdPostSettings), typeof(PublisherPostSettings), new PropertyMetadata(new FdPostSettings()));

        public static void SetFdPostSettings(DependencyObject element, FdPostSettings value)
        {
            element.SetValue(FdPostSettingsProperty, value);
        }

        public static FdPostSettings GetFdPostSettings(DependencyObject element)
        {
            return (FdPostSettings) element.GetValue(FdPostSettingsProperty);
        }


        public static readonly DependencyProperty GdPostSettingsProperty = DependencyProperty.RegisterAttached(
            "GdPostSettings", typeof(GdPostSettings), typeof(PublisherPostSettings), new PropertyMetadata(new GdPostSettings()));

        public static void SetGdPostSettings(DependencyObject element, GdPostSettings value)
        {
            element.SetValue(GdPostSettingsProperty, value);
        }

        public static GdPostSettings GetGdPostSettings(DependencyObject element)
        {
            return (GdPostSettings) element.GetValue(GdPostSettingsProperty);
        }

        public static readonly DependencyProperty TumberPostSettingsProperty = DependencyProperty.RegisterAttached(
            "TumberPostSettings", typeof(TumberPostSettings), typeof(PublisherPostSettings), new PropertyMetadata(new TumberPostSettings()));

        public static void SetTumberPostSettings(DependencyObject element, TumberPostSettings value)
        {
            element.SetValue(TumberPostSettingsProperty, value);
        }

        public static TumberPostSettings GetTumberPostSettings(DependencyObject element)
        {
            return (TumberPostSettings) element.GetValue(TumberPostSettingsProperty);
        }


        public static readonly DependencyProperty LdPostSettingsProperty = DependencyProperty.RegisterAttached(
            "LdPostSettings", typeof(LdPostSettings), typeof(PublisherPostSettings), new PropertyMetadata(new LdPostSettings()));

        public static void SetLdPostSettings(DependencyObject element, LdPostSettings value)
        {
            element.SetValue(LdPostSettingsProperty, value);
        }

        public static LdPostSettings GetLdPostSettings(DependencyObject element)
        {
            return (LdPostSettings) element.GetValue(LdPostSettingsProperty);
        }

        public static readonly DependencyProperty TdPostSettingsProperty = DependencyProperty.RegisterAttached(
            "TdPostSettings", typeof(TdPostSettings), typeof(PublisherPostSettings), new PropertyMetadata(new TdPostSettings()));

        public static void SetTdPostSettings(DependencyObject element, TdPostSettings value)
        {
            element.SetValue(TdPostSettingsProperty, value);
        }

        public static TdPostSettings GetTdPostSettings(DependencyObject element)
        {
            return (TdPostSettings) element.GetValue(TdPostSettingsProperty);
        }
        
    }
}