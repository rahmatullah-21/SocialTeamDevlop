using System;
using System.Windows.Controls;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;

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

        public PublisherPostSettings PublisherPostSettings { get; set; }

        public PostGeneralSettings(PublisherPostSettings publisherPostSettings) : this()
        {
            PublisherPostSettings = publisherPostSettings;
            MainGrid.DataContext = publisherPostSettings.GeneralPostSettings;
        }

        private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PublisherPostSettings.GeneralPostSettings.ExpireDate < DateTime.Today)
            {
                Dialog.ShowDialog("Warning", "Expire date should be greater than today!");
                PublisherPostSettings.GeneralPostSettings.ExpireDate = DateTime.Now;
            }
                        
        }
    }
}
