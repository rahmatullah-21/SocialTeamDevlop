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

        private void ToggleCampaignStatus_OnClick(object sender, RoutedEventArgs e)
        {
            string onLabel = ToggleCampaignStatus.OnLabel;
            switch (onLabel)
            {
                case "Completed":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus = "Stopped";
                    break;
                case "Stopped":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus = "Active";
                    break;
                case "Active":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus = "Paused";
                    break;
                case "Paused":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus = "Completed";
                    break;

            }
        }

        private void ComboCampaignList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                PublisherCreateCampaignViewModel.PublisherCreateCampaignModel = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>
                        (ConstantVariable.GetOtherDir() + "\\Campaign.bin").FirstOrDefault(x => x.CampaignName == ComboCampaignList.SelectedItem.ToString());
            }
            catch (System.Exception ex)
            {

                PublisherCreateCampaignViewModel.PublisherCreateCampaignModel =new PublisherCreateCampaignModel();
            }
        }
    }
}
