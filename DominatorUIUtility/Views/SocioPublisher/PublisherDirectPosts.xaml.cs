using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherDirectPosts.xaml
    /// </summary>
    public partial class PublisherDirectPosts : UserControl , INotifyPropertyChanged
    {
        public PublisherDirectPosts()
        {
            InitializeComponent();
            _currentPublisherDirectPosts = this;
            DirectPost.DataContext = PublisherDirectPostsViewModel;
           


        }

        private static PublisherDirectPosts _currentPublisherDirectPosts = null;

        public static PublisherDirectPosts GetSingeltonPublisherDirectPosts()
        {
            return _currentPublisherDirectPosts ?? (_currentPublisherDirectPosts = new PublisherDirectPosts());
        }

        private PublisherDirectPostsViewModel _publisherDirectPostsViewModel=new PublisherDirectPostsViewModel();

        public PublisherDirectPostsViewModel PublisherDirectPostsViewModel
        {
            get
            {
                return _publisherDirectPostsViewModel;
            }
            set
            {
                _publisherDirectPostsViewModel = value;
                OnPropertyChanged(nameof(PublisherDirectPostsViewModel));
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
