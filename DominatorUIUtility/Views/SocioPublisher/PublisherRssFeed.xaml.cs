using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherRssFeed.xaml
    /// </summary>
    public partial class PublisherRssFeed : UserControl,INotifyPropertyChanged
    {  
        private PublisherRssFeed()
        {
            InitializeComponent();           
        }

        private PublisherRssFeed(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {           
            PublisherRssFeedViewModel = new PublisherRssFeedViewModel(tabItemsControl);
            tabItemsControl.PublisherRssFeedViewModel = PublisherRssFeedViewModel;
            MainGrid.DataContext = PublisherRssFeedViewModel;
        }

        private static PublisherRssFeed _instance = null;

        public static PublisherRssFeed GetPublisherRssFeed(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return _instance ?? (_instance = new PublisherRssFeed(tabItemsControl));
        }

        private PublisherRssFeedViewModel _publisherRssFeedViewModel = new PublisherRssFeedViewModel();

        public PublisherRssFeedViewModel PublisherRssFeedViewModel
        {
            get
            {
                return _publisherRssFeedViewModel;
            }
            set
            {
                if (_publisherRssFeedViewModel == value)
                    return;
                _publisherRssFeedViewModel = value;
                OnPropertyChanged(nameof(PublisherRssFeedViewModel));
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
