using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherDirectPosts.xaml
    /// </summary>
    public partial class PublisherDirectPosts : UserControl, INotifyPropertyChanged
    {
        private PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl;
        public PublisherDirectPosts()
        {
            InitializeComponent();
            instance = this;
        }
        public PublisherDirectPosts(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            this.tabItemsControl = tabItemsControl;
            PublisherDirectPostsViewModel = new PublisherDirectPostsViewModel(tabItemsControl);
            tabItemsControl.publisherDirectPostsViewModel = PublisherDirectPostsViewModel;
            DirectPost.DataContext = PublisherDirectPostsViewModel;

        }

        private static PublisherDirectPosts instance;

        public static PublisherDirectPosts GetPublisherDirectPosts(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return instance ?? (instance = new PublisherDirectPosts(tabItemsControl));
        }

        private PublisherDirectPostsViewModel _publisherDirectPostsViewModel;

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

        private void PublisherDirectPosts_OnLoaded(object sender, RoutedEventArgs e)
        {
            PostContentControl.SetMedia();
        }
    }
}
