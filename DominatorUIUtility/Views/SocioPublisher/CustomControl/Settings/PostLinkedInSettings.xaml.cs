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
using DominatorHouseCore.Models.SocioPublisher.Settings;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    /// Interaction logic for PostLinkedInSettings.xaml
    /// </summary>
    public partial class PostLinkedInSettings : UserControl
    {
        public PostLinkedInSettings()
        {
            InitializeComponent();
        }

        public PostLinkedInSettings(PublisherPostSettings publisherPostSettings):this()
        {
            MainGrid.DataContext = publisherPostSettings.LdPostSettings;
        }
    }
}
