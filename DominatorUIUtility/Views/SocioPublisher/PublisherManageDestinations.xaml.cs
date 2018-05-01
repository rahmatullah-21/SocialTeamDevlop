using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManageDestinations.xaml
    /// </summary>
    public partial class PublisherManageDestinations : UserControl, INotifyPropertyChanged
    {
        private PublisherManageDestinations()
        {
            InitializeComponent();
            ManageDestination.DataContext = PublisherManageDestinationViewModel;
        }

        public PublisherManageDestinationViewModel PublisherManageDestinationViewModel
        {
            get
            {
                return _publisherManageDestinationViewModel;
            }
            set
            {
                _publisherManageDestinationViewModel = value;
                OnPropertyChanged(nameof(PublisherManageDestinationViewModel));
            }
        }

        private static PublisherManageDestinations _indexPage;
        private PublisherManageDestinationViewModel _publisherManageDestinationViewModel = new PublisherManageDestinationViewModel();

        public static PublisherManageDestinations Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherManageDestinations());


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
