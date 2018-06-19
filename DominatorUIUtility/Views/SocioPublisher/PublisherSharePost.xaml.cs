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
    /// Interaction logic for PublisherSharePost.xaml
    /// </summary>
    public partial class PublisherSharePost : UserControl,INotifyPropertyChanged
    {
        private PublisherSharePost()
        {
            InitializeComponent();
        }
        
        public PublisherSharePost(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl):this()
        {
            PublisherSharePostViewModel = new PublisherSharePostViewModel(tabItemsControl);
            tabItemsControl.PublisherSharePostViewModel = PublisherSharePostViewModel;
            MainGrid.DataContext = PublisherSharePostViewModel;

        }
        private static PublisherSharePost _instance = null;

        public static PublisherSharePost GetPublisherSharePost(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return _instance ?? (_instance = new PublisherSharePost(tabItemsControl));
        }
        private PublisherSharePostViewModel _publisherSharePostViewModel;

        public PublisherSharePostViewModel PublisherSharePostViewModel
        {
            get
            {
                return _publisherSharePostViewModel;
            }
            set
            {
                if (_publisherSharePostViewModel == value)
                    return;
                _publisherSharePostViewModel = value;
                OnPropertyChanged(nameof(PublisherSharePostViewModel));
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
