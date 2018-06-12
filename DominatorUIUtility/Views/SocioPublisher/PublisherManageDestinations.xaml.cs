using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManageDestinations.xaml
    /// </summary>
    public partial class PublisherManageDestinations : UserControl, INotifyPropertyChanged
    {
        private Visibility HeaderVisiblity { get; set; } = Visibility.Visible;
        private PublisherManageDestinations()
        {
            InitializeComponent();
            ManageDestination.DataContext = PublisherManageDestinationViewModel;
            
        }
        public PublisherManageDestinations(Visibility HeaderVisiblity):this()
        {
            PublisherManageDestinationViewModel.HeaderVisibility = HeaderVisiblity;
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


        private PublisherManageDestinationViewModel _publisherManageDestinationViewModel = new PublisherManageDestinationViewModel();

        private static PublisherManageDestinations _indexPage;
        public static PublisherManageDestinations Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherManageDestinations());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var destination = ((FrameworkElement)sender).DataContext as PublisherManageDestinationModel;

            if (destination == null)
                return;

            PublisherCreateDestination.Instance.PublisherCreateDestinationsViewModel.EditDestinationId =
                destination.DestinationId;

            PublisherCreateDestination.Instance.PublisherCreateDestinationsViewModel.IsSavedDestination = true;

            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                = PublisherCreateDestination.Instance;
        }
    }
}
