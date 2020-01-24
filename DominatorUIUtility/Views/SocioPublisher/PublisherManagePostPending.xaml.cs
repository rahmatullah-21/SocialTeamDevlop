using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using LegionUIUtility.ViewModel.SocioPublisher;

namespace LegionUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePostPending.xaml
    /// </summary>
    public partial class PublisherManagePostPending : UserControl, INotifyPropertyChanged
    {
        private PublisherManagePostPending()
        {
            InitializeComponent();
            PendingPostLists.DataContext = PublisherManagePostPendingViewModel;
        }

        private static PublisherManagePostPending _publisherManagePostPending;

        public static PublisherManagePostPending GetPublisherManagePostPending()
            => _publisherManagePostPending ?? (_publisherManagePostPending = new PublisherManagePostPending());

        private PublisherManagePostPendingViewModel _publisherManagePostPendingViewModel = new PublisherManagePostPendingViewModel();
        public PublisherManagePostPendingViewModel PublisherManagePostPendingViewModel
        {
            get
            {
                return _publisherManagePostPendingViewModel;
            }
            set
            {
                if (_publisherManagePostPendingViewModel == value)
                    return;
                _publisherManagePostPendingViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostPendingViewModel));
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
