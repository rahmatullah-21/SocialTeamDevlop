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
using DominatorHouseCore.Models;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for CampaignsAdvanceSetting.xaml
    /// </summary>
    public partial class CampaignsAdvanceSetting : UserControl
    {
        public CampaignsAdvanceSetting()
        {
            InitializeComponent();
            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title=FindResource("langGeneral").ToString(),
                    Content=new Lazy<UserControl>(General.GetSingeltonGeneralObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langFacebook").ToString(),
                    Content=new Lazy<UserControl>(Facebook.GetSingeltonFacebookObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langGooglePlus").ToString(),
                    Content=new Lazy<UserControl>(GooglePlus.GetSingeltonGooglePlusObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langPinterest").ToString(),
                    Content=new Lazy<UserControl>(Pinterest.GetSingeltonPinterestObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langTwitter").ToString(),
                    Content=new Lazy<UserControl>(Twitter.GetSingletonTwitterObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langInstagram").ToString(),
                    Content=new Lazy<UserControl>(Instagram.GetSingeltonInstagramObject)
                } ,
                new TabItemTemplates
                {
                    Title=FindResource("DHlangTumblr").ToString(),
                    Content=new Lazy<UserControl>(Tumblr.GetSingeltonTumblr)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langErrorHandling").ToString(),
                    Content=new Lazy<UserControl>(ErrorHandling.GetSingeltonErrorHandlingObject)
                }

            };
            CampaignsAdvanceSettingTab.ItemsSource = TabItems;
        }
    }
}
