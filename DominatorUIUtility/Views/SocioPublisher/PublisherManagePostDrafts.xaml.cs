using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePostDrafts.xaml
    /// </summary>
    public partial class PublisherManagePostDrafts : UserControl, INotifyPropertyChanged
    {
        private PublisherManagePostDrafts()
        {
            InitializeComponent();
            DraftPostList.DataContext = PublisherManagePostDraftsViewModel;
        }

        private static PublisherManagePostDrafts _publisherManagePostDrafts;

        public static PublisherManagePostDrafts GetPublisherManagePostDrafts() 
            => _publisherManagePostDrafts ?? (_publisherManagePostDrafts = new PublisherManagePostDrafts());


        private PublisherManagePostDraftsViewModel _publisherManagePostDraftsViewModel = new PublisherManagePostDraftsViewModel();
        public PublisherManagePostDraftsViewModel PublisherManagePostDraftsViewModel
        {
            get
            {
                return _publisherManagePostDraftsViewModel;
            }
            set
            {
                if (_publisherManagePostDraftsViewModel == value)
                    return;

                _publisherManagePostDraftsViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostDraftsViewModel));
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
