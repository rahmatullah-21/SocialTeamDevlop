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
    public partial class OtherTools : UserControl
    {
        public OtherTools()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title =FindResource("LangKeyMediaGenerator").ToString(),
                   Content = new Lazy<UserControl>(MediaGenerator.GetMediaGeneratorInstance)
                },
              
            };

            OtherToolsTabs.ItemsSource = items;
        }
    }
}
