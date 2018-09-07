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
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.ConfigControl;
using DominatorUIUtility.CustomControl;

namespace DominatorHouse.OtherConfiguration
{
    /// <summary>
    /// Interaction logic for MigrationFromOldDominator.xaml
    /// </summary>
    public partial class MigrationFromOldDominator : UserControl
    {
        public MigrationFromOldDominator()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title =FindResource("LangKeyInstagram").ToString(),
                   Content = new Lazy<UserControl>(()=>new Migration(SocialNetworks.Instagram))
                },
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyFacebook").ToString(),
                  Content = new Lazy<UserControl>(()=>new Migration(SocialNetworks.Facebook))
                },
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyReddit").ToString(),
                    Content = new Lazy<UserControl>(()=>new Migration(SocialNetworks.Reddit))
                },
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyLinkedIn").ToString(),
                    Content = new Lazy<UserControl>(()=>new Migration(SocialNetworks.LinkedIn))
                },
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyTwitter").ToString(),
                    Content = new Lazy<UserControl>(()=>new Migration(SocialNetworks.Twitter))
                },
               
            };

            MigrationFromOldDominatorTab.ItemsSource = items;
        }
    }
}
