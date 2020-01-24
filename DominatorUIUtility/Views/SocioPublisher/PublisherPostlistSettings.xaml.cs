using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;

namespace LegionUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherPostlistSettings.xaml
    /// </summary>
    public partial class PublisherPostlistSettings : UserControl , INotifyPropertyChanged
    {
        public PublisherPostlistSettings()
        {
            InitializeComponent();           
        }

        public PublisherPostlistSettings(PublisherPostlistSettingsModel publisherPostlistSettingsModel) 
        {
            InitializeComponent();
            PublisherPostlistSettingsModel = publisherPostlistSettingsModel;
            PostlistSettings.DataContext = PublisherPostlistSettingsModel;        
        }

        private PublisherPostlistSettingsModel _publisherPostlistSettingsModel = new PublisherPostlistSettingsModel();

        public PublisherPostlistSettingsModel PublisherPostlistSettingsModel
        {
            get
            {
                return _publisherPostlistSettingsModel;
            }
            set
            {
                _publisherPostlistSettingsModel = value;
                OnPropertyChanged(nameof(PublisherPostlistSettingsModel));
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
