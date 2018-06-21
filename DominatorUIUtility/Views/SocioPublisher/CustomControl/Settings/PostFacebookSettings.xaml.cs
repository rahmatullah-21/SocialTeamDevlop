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
    /// Interaction logic for PostFacebookSettings.xaml
    /// </summary>
    public partial class PostFacebookSettings : UserControl
    {
        public PostFacebookSettings()
        {
            InitializeComponent();
        }

        public PostFacebookSettings(PublisherPostSettings publisherPostSettings):this()
        {
            MainGrid.DataContext = publisherPostSettings.FdPostSettings;
        }
    }
}
