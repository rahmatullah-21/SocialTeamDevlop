using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using LegionUIUtility.ViewModel.SocioPublisher;

namespace LegionUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherAddCustomDestination.xaml
    /// </summary>
    public partial class PublisherAddCustomDestination : UserControl, INotifyPropertyChanged
    {
        private PublisherAddCustomDestination()
        {
            InitializeComponent();
            CustomDestination.DataContext = PublisherCustomDestinationViewModel;
        }

        private static PublisherAddCustomDestination _publisherAddCustomDestination;


        public static PublisherAddCustomDestination GetPublisherAddCustomDestination(ObservableCollection<PublisherCustomDestinationModel> alreadySavedDestination)
        {
            if (_publisherAddCustomDestination == null)
                _publisherAddCustomDestination = new PublisherAddCustomDestination();

            _publisherAddCustomDestination.PublisherCustomDestinationViewModel.LstCustomDestination =alreadySavedDestination;

            _publisherAddCustomDestination.CustomDestination.DataContext = _publisherAddCustomDestination.PublisherCustomDestinationViewModel;

            return _publisherAddCustomDestination;
        }


        private PublisherCustomDestinationViewModel _publisherCustomDestinationViewModel = new PublisherCustomDestinationViewModel();

        public PublisherCustomDestinationViewModel PublisherCustomDestinationViewModel
        {
            get
            {
                return _publisherCustomDestinationViewModel;
            }
            set
            {

                if (_publisherCustomDestinationViewModel == value)
                    return;
                _publisherCustomDestinationViewModel = value;
                OnPropertyChanged(nameof(PublisherCustomDestinationViewModel));
            }
        }

        public void ResetCurrectObject() 
            => _publisherAddCustomDestination = new PublisherAddCustomDestination();


        public ObservableCollection<PublisherCustomDestinationModel> GetSavedCustomDestination() 
            => PublisherCustomDestinationViewModel.LstCustomDestination;


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
