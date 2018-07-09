using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using DominatorUIUtility.ConfigControl;

namespace DominatorHouse.OtherConfiguration
{
    /// <summary>
    /// Interaction logic for ThirdPartyServices.xaml
    /// </summary>
    public partial class ThirdPartyServices : UserControl
    {
        private ThirdPartyServices()
        {
            InitializeComponent();
            instance = this;
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyUrlShortnerServices").ToString(),
                   Content = new Lazy<UserControl>(URLShortnerServices.GetSingeltonUrlShortnerServices)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyCaptchaServices").ToString(),
                     Content = new Lazy<UserControl>(CaptchaServices.GetSingeltonCaptchaServices)
                },
            };
            ThirdPartyService.ItemsSource = items;
        }

        private static ThirdPartyServices instance;
        public static UserControl GetSingeltonThirdPartyServices()
        {
            return instance ?? (instance = new ThirdPartyServices());
        }
    }
}
