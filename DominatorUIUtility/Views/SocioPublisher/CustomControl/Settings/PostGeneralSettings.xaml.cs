using System.Windows.Controls;
using DominatorHouseCore.Models.SocioPublisher.Settings;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    /// Interaction logic for PostGeneralSettings.xaml
    /// </summary>
    public partial class PostGeneralSettings : UserControl
    {
        public PostGeneralSettings()
        {
            InitializeComponent();
        }

        public PostGeneralSettings(PublisherPostSettings PublisherPostSettings):this()
        {
            MainGrid.DataContext = PublisherPostSettings.GeneralPostSettings;
        }
    }
}
