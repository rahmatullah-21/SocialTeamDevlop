using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for Pinterest.xaml
    /// </summary>
    public partial class Pinterest : UserControl
    {
        private Pinterest()
        {
            InitializeComponent();
            MainGrid.DataContext = PinterestViewModel;
        }
        static Pinterest ObjPinterest = null;
        public static Pinterest GetSingeltonPinterestObject()
        {
            if (ObjPinterest == null)
                ObjPinterest = new Pinterest();
            return ObjPinterest;
        }
        private PinterestViewModel _pinterestViewModel = new PinterestViewModel();

        public PinterestViewModel PinterestViewModel
        {
            get
            {
                return _pinterestViewModel;
            }
            set
            {
                _pinterestViewModel = value;
                OnPropertyChanged(nameof(PinterestViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Pinterest_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var pinterestModel = GenericFileManager.GetModuleDetails<PinterestModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            PinterestViewModel.PinterestModel = pinterestModel ?? PinterestViewModel.PinterestModel;
        }
    }
}
