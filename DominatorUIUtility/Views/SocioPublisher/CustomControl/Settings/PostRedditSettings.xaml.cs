using System.Windows.Controls;
using DominatorHouseCore.Models.SocioPublisher.Settings;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    /// Interaction logic for PostRedditSettings.xaml
    /// </summary>
    public partial class PostRedditSettings : UserControl
    {
        public PostRedditSettings()
        {
            InitializeComponent();
        }

        public PostRedditSettings(PublisherPostSettings publisherPostSettings):this()
        {
            MainGrid.DataContext = publisherPostSettings.RedditPostSetting;
        }
    }
}
