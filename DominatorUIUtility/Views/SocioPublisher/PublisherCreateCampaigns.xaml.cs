using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherCreateCampaigns.xaml
    /// </summary>
    public partial class PublisherCreateCampaigns : UserControl , INotifyPropertyChanged
    {
        private PublisherCreateCampaignViewModel _publisherCreateCampaignViewModel = new PublisherCreateCampaignViewModel();

        public PublisherCreateCampaigns()
        {
            InitializeComponent();
            CreateCampaign.DataContext = PublisherCreateCampaignViewModel;
            currentObject = this;
        }
        private static PublisherCreateCampaigns currentObject;

        public static PublisherCreateCampaigns GetSingeltonPublisherCreateCampaigns()
        {
            return currentObject?? (currentObject=new PublisherCreateCampaigns());
        }
        public PublisherCreateCampaignViewModel PublisherCreateCampaignViewModel
        {
            get
            {
                return _publisherCreateCampaignViewModel;
            }
            set
            {
                if(_publisherCreateCampaignViewModel == value)
                    return;
                _publisherCreateCampaignViewModel = value;
                OnPropertyChanged(nameof(PublisherCreateCampaignViewModel));
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
