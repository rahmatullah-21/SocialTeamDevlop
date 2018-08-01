using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    /// Interaction logic for PostGeneralSettings.xaml
    /// </summary>
    public partial class PostGeneralSettings : UserControl,INotifyPropertyChanged
    {
        private PublisherPostSettings _publisherPostSettings;

        private PostGeneralSettings()
        {
            InitializeComponent();
        }


        public PublisherPostSettings PublisherPostSettings
        {
            get
            {
                return _publisherPostSettings;
            }
            set
            {
                _publisherPostSettings = value;
                OnPropertyChanged(nameof(PublisherPostSettings));
            }
        }

        public PostGeneralSettings(PublisherPostSettings publisherPostSettings) : this()
        {
            PublisherPostSettings = publisherPostSettings;
            MainGrid.DataContext = PublisherPostSettings.GeneralPostSettings;        
        }

     

        private void DatePicker_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PublisherPostSettings.GeneralPostSettings.ExpireDate < DateTime.Today)
            {
                Dialog.ShowDialog("Warning", "Expire date should be greater than today!");
                PublisherPostSettings.GeneralPostSettings.ExpireDate = DateTime.Now;
            }
                        
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
