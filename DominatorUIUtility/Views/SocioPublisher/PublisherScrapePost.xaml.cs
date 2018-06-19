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
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
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
        private static PublisherScrapePost _instance = null;

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
