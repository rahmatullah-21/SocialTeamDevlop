using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePostPublished.xaml
    /// </summary>
    public partial class PublisherManagePostPublished : UserControl, INotifyPropertyChanged
    {
        public PublisherManagePostPublished()
        {
            InitializeComponent();
            PublishedPostList.DataContext = PublisherManagePostPublishedViewModel;
        }

        private PublisherManagePostPublishedViewModel _publisherManagePostPublishedViewModel = new PublisherManagePostPublishedViewModel();
        public PublisherManagePostPublishedViewModel PublisherManagePostPublishedViewModel
        {
            get
            {
                return _publisherManagePostPublishedViewModel;
            }
            set
            {
                if (_publisherManagePostPublishedViewModel == value)
                    return;
                _publisherManagePostPublishedViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostPublishedViewModel));
            }
        }

        //private static PublisherManagePostPublished _instance;


        //public static PublisherManagePostPublished Instance { get; set; }
        //    = _instance ?? (_instance = new PublisherManagePostPublished());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MediaViewer_OnNextImage(object sender, RoutedEventArgs e)
            => PublisherManagePostPublishedViewModel.NextImage(sender);


        private void MediaViewer_OnPreviousImage(object sender, RoutedEventArgs e)
            => PublisherManagePostPublishedViewModel.PreviousImage(sender);
    }
}
