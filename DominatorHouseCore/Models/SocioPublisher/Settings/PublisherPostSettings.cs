using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [ProtoContract]
    public class PublisherPostSettings 
    {

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
        [ProtoMember(1)]
        public GeneralPostSettings GeneralPostSettings { get; set; }=new GeneralPostSettings();
        [ProtoMember(2)]
        public FdPostSettings FdPostSettings { get; set; }=new FdPostSettings();
        [ProtoMember(3)]
        public GdPostSettings GdPostSettings { get; set; }=new GdPostSettings();
        [ProtoMember(4)]
        public TdPostSettings TdPostSettings { get; set; }=new TdPostSettings();
        [ProtoMember(5)]
        public LdPostSettings LdPostSettings { get; set; }=new LdPostSettings();
        [ProtoMember(6)]
        public TumberPostSettings TumberPostSettings { get; set; }=new TumberPostSettings();
    }
}