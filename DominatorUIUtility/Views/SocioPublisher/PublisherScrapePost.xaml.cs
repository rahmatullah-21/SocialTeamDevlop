using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using LegionUIUtility.ViewModel.SocioPublisher;

namespace LegionUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherScrapePost.xaml
    /// </summary>
    public partial class PublisherScrapePost : UserControl,INotifyPropertyChanged
    {
        public PublisherScrapePost()
        {
            InitializeComponent();
        }
        public PublisherScrapePost(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl):this()
        {
            PublisherScrapePostViewModel = new PublisherScrapePostViewModel(tabItemsControl);
            tabItemsControl.PublisherScrapePostViewModel = PublisherScrapePostViewModel;
            MainGrid.DataContext = PublisherScrapePostViewModel;

        }
        private static PublisherScrapePost _instance;

        public static PublisherScrapePost GetPublisherScrapePost(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return _instance ?? (_instance = new PublisherScrapePost(tabItemsControl));
        }
        private PublisherScrapePostViewModel _publisherScrapePostViewModel;

        public PublisherScrapePostViewModel PublisherScrapePostViewModel
        {
            get
            {
                return _publisherScrapePostViewModel;
            }
            set
            {
                if (_publisherScrapePostViewModel == value)
                    return;
                _publisherScrapePostViewModel = value;
                OnPropertyChanged(nameof(PublisherScrapePostViewModel));
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
