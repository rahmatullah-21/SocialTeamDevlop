using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouse.OtherConfiguration;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ConfigControl;
using DominatorUIUtility.CustomControl;


namespace Socinator.Social.OtherConfiguration
{
    /// <summary>
    /// Interaction logic for OtherConfigurationTab.xaml
    /// </summary>
    public partial class OtherConfigurationTab : UserControl
    {
        public OtherConfigurationTab()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title =FindResource("LangKeySoftwareSettings").ToString(),
                   Content = new Lazy<UserControl>(SoftwareSettings.GetSingeltonObjectSoftwareSettings)
                },
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyMacroS").ToString(),
                  Content = new Lazy<UserControl>(SocinatorMacros.GetSingeltonSocinatorMacros)
                },
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyThirdPartyServices").ToString(),
                    Content = new Lazy<UserControl>(ThirdPartyServices.GetSingeltonThirdPartyServices)
                }
                ,
                new TabItemTemplates
                {
                    Title = "LangKeyMigrationFromOldDominator".FromResourceDictionary(),
                    Content=new Lazy<UserControl>(()=>new MigrationFromOldDominator())
                },
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyQuora").ToString(),
                    Content = new Lazy<UserControl>(Quora.GetSingeltonObjectQuora)
                },
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyLinkedIn").ToString(),
                    Content = new Lazy<UserControl>(LinkedIn.GetSingeltonObjectLinkedIn)
                },
                //new TabItemTemplates
                //{
                //    Title=FindResource("langFacebook").ToString(),
                //    Content = new Lazy<UserControl>(Facebook.GetSingeltonObjectFacebook)
                //},
                //new TabItemTemplates
                //{
                //    Title=FindResource("langInstagram").ToString(),
                //    Content = new Lazy<UserControl>(Instagram.GetSingeltonObjectInstagram)
                //},
                //new TabItemTemplates
                //{
                //    Title=FindResource("DHlangTwitter").ToString(),
                //    Content = new Lazy<UserControl>(Twitter.GetSingeltonObjectTwitter)
                //},
                //new TabItemTemplates
                //{
                //    Title=FindResource("DHlangPinterest").ToString(),
                //    Content = new Lazy<UserControl>(Pinterest.GetSingeltonObjectPinterest)
                //},
                //new TabItemTemplates
                //{
                //    Title=FindResource("DHlangTumblr").ToString(),
                //    Content = new Lazy<UserControl>(Tumblr.GetSingeltonObjectTumblr)
                //},
                //new TabItemTemplates
                //{
                //    Title=FindResource("DHlangYoutube").ToString(),
                //    Content = new Lazy<UserControl>(Youtube.GetSingeltonObjectYoutube)
                //},
                //new TabItemTemplates
                //{
                //    Title=FindResource("DHlangEmailNotifications").ToString(),
                //    Content = new Lazy<UserControl>(EmailNotifications.GetSingeltonObjectEmailNotifications)
                //}
            };

            OtherConfigurationTabs.ItemsSource = items;
        }
    }
}
