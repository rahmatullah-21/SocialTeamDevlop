using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherCreateDestination.xaml
    /// </summary>
    public partial class PublisherCreateDestination : UserControl ,INotifyPropertyChanged
    {
        private PublisherCreateDestination()
        {
            InitializeComponent();
            CreateDestination.DataContext = PublisherCreateDestinationsViewModel;
        }

        public PublisherCreateDestinationsViewModel PublisherCreateDestinationsViewModel
        {
            get
            {
                return _publisherCreateDestinationsViewModel;
            }
            set
            {
                _publisherCreateDestinationsViewModel = value;
                OnPropertyChanged(nameof(PublisherCreateDestinationsViewModel));
            }
        }

        private static PublisherCreateDestination _indexPage;
        private PublisherCreateDestinationsViewModel _publisherCreateDestinationsViewModel = new PublisherCreateDestinationsViewModel();

        public static PublisherCreateDestination Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherCreateDestination());
      
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PublisherCreateDestination_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!PublisherCreateDestinationsViewModel.IsSavedDestination)
                return;
            PublisherCreateDestinationsViewModel = new PublisherCreateDestinationsViewModel();
            CreateDestination.DataContext = PublisherCreateDestinationsViewModel;
        }
    }
}
