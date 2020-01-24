using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace LegionUIUtility.Behaviours
{
    public class NetworkAvailablity
    {       
        #region Facebook

        public static readonly DependencyProperty FbElementVisibleProperty = DependencyProperty.RegisterAttached(
            "FbElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Facebook)));

        public static void SetFbElementVisible(DependencyObject element, Visibility value)
        {
            element.SetValue(FbElementVisibleProperty, value);
        }

        public static Visibility GetFbElementVisible(DependencyObject element)
        {
            return (Visibility) element.GetValue(FbElementVisibleProperty);
        }


        #endregion

        //#region Instagram

        //public static readonly DependencyProperty IgElementVisibleProperty = DependencyProperty.RegisterAttached(
        //    "IgElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Instagram)));

        //public static void SetIgElementVisible(DependencyObject element, Visibility value)
        //{
        //    element.SetValue(IgElementVisibleProperty, value);
        //}

        //public static Visibility GetIgElementVisible(DependencyObject element)
        //{
        //    return (Visibility) element.GetValue(IgElementVisibleProperty);
        //}


        //#endregion

        //#region Twitter


        //public static readonly DependencyProperty TwtElementVisibleProperty = DependencyProperty.RegisterAttached(
        //    "TwtElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Twitter)));

        //public static void SetTwtElementVisible(DependencyObject element, Visibility value)
        //{
        //    element.SetValue(TwtElementVisibleProperty, value);
        //}

        //public static Visibility GetTwtElementVisible(DependencyObject element)
        //{
        //    return (Visibility) element.GetValue(TwtElementVisibleProperty);
        //}

        //#endregion

        //#region LinkedIn

        //public static readonly DependencyProperty LdElementVisibleProperty = DependencyProperty.RegisterAttached(
        //    "LdElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.LinkedIn)));

        //public static void SetLdElementVisible(DependencyObject element, Visibility value)
        //{
        //    element.SetValue(LdElementVisibleProperty, value);
        //}

        //public static Visibility GetLdElementVisible(DependencyObject element)
        //{
        //    return (Visibility) element.GetValue(LdElementVisibleProperty);
        //}

        //#endregion

        //#region Tumblr

        //public static readonly DependencyProperty TumblrElementVisibleProperty = DependencyProperty.RegisterAttached(
        //    "TumblrElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Tumblr)));

        //public static void SetTumblrElementVisible(DependencyObject element, Visibility value)
        //{
        //    element.SetValue(TumblrElementVisibleProperty, value);
        //}

        //public static Visibility GetTumblrElementVisible(DependencyObject element)
        //{
        //    return (Visibility) element.GetValue(TumblrElementVisibleProperty);
        //}

        //#endregion

        //#region Pinterest

        //public static readonly DependencyProperty PdElementVisibleProperty = DependencyProperty.RegisterAttached(
        //    "PdElementVisible", typeof(Visibility), typeof(NetworkAvailablity), new PropertyMetadata(FeatureFlags.Check(SocialNetworks.Pinterest)));

        //public static void SetPdElementVisible(DependencyObject element, Visibility value)
        //{
        //    element.SetValue(PdElementVisibleProperty, value);
        //}

        //public static Visibility GetPdElementVisible(DependencyObject element)
        //{
        //    return (Visibility) element.GetValue(PdElementVisibleProperty);
        //}

        //#endregion
    }
}