using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePostPending.xaml
    /// </summary>
    public partial class PublisherManagePostPending : UserControl , INotifyPropertyChanged
    {
        public PublisherManagePostPending()
        {
            InitializeComponent();
            PendingPostLists.DataContext = PublisherManagePostPendingViewModel;
        }

        private PublisherManagePostPendingViewModel _publisherManagePostPendingViewModel = new PublisherManagePostPendingViewModel();
        public PublisherManagePostPendingViewModel PublisherManagePostPendingViewModel
        {
            get
            {
                return _publisherManagePostPendingViewModel;
            }
            set
            {
                if(_publisherManagePostPendingViewModel== value)
                    return;

                _publisherManagePostPendingViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostPendingViewModel));
            }
        }

        //private static PublisherManagePostPending _instance;
       

        //public static PublisherManagePostPending Instance { get; set; }
        //    = _instance ?? (_instance = new PublisherManagePostPending());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MediaViewer_OnNextImage(object sender, RoutedEventArgs e)
            => PublisherManagePostPendingViewModel.NextImage(sender);


        private void MediaViewer_OnPreviousImage(object sender, RoutedEventArgs e)
            => PublisherManagePostPendingViewModel.PreviousImage(sender);
    }
}
