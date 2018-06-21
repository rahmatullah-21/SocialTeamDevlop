using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherCreateCampaigns.xaml
    /// </summary>
    public partial class PublisherCreateCampaigns : UserControl, INotifyPropertyChanged
    {
        private PublisherCreateCampaignViewModel _publisherCreateCampaignViewModel = new PublisherCreateCampaignViewModel();

        private PublisherCreateCampaigns()
        {
            InitializeComponent();
            CreateCampaign.DataContext = PublisherCreateCampaignViewModel;
            _currentObject = this;
        }
    
        private static PublisherCreateCampaigns _currentObject;

        public static PublisherCreateCampaigns GetSingeltonPublisherCreateCampaigns()
        {
            return _currentObject ?? (_currentObject = new PublisherCreateCampaigns());
        }
        public static PublisherCreateCampaigns Instance { get; set; }
            = _currentObject ?? (_currentObject = new PublisherCreateCampaigns());
        public PublisherCreateCampaignViewModel PublisherCreateCampaignViewModel
        {
            get
            {
                return _publisherCreateCampaignViewModel;
            }
            set
            {
                if (_publisherCreateCampaignViewModel == value)
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

        private void ToggleCampaignStatus_OnClick(object sender, RoutedEventArgs e)
        {
            string onLabel = ToggleCampaignStatus.OnLabel;
            switch (onLabel)
            {
                case "Completed":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus = PublisherCampaignStatus.Stopped;
                    break;
                case "Stopped":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus = PublisherCampaignStatus.Active;
                    break;
                case "Active":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus = PublisherCampaignStatus.Paused;
                    break;
                case "Paused":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus = PublisherCampaignStatus.Completed;
                    break;

            }
        }

    }
}
