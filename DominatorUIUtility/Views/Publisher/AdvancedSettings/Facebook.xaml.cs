using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using LegionUIUtility.ViewModel.SocioPublisher.AdvancedSettings;
using LegionUIUtility.Views.SocioPublisher;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace LegionUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for Facebook.xaml
    /// </summary>
    public partial class Facebook : UserControl, INotifyPropertyChanged
    {
        private readonly IGenericFileManager _genericFileManager;
        public Facebook()
        {
            _genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = FacebookViewModel;
            FacebookViewModel.FacebookModel.CampaignId = "";
        }

        static Facebook ObjFacebook;

        public event PropertyChangedEventHandler PropertyChanged;

        public static Facebook GetSingeltonFacebookObject()
        {
            if (ObjFacebook == null)
                ObjFacebook = new Facebook();
            return ObjFacebook;
        }


        private FacebookViewModel _facebookViewModel = new FacebookViewModel();

        public FacebookViewModel FacebookViewModel
        {
            get
            {
                return _facebookViewModel;
            }
            set
            {
                _facebookViewModel = value;
                OnPropertyChanged(nameof(FacebookViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void Facebook_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var facebookModel = _genericFileManager.GetModuleDetails<FacebookModel>
                (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook))
                .FirstOrDefault(x => x.CampaignId == campaignId);

            FacebookViewModel.FacebookModel = facebookModel ?? (new FacebookModel());

            if (!FacebookViewModel.FacebookModel.IsPostAsPage && !FacebookViewModel.FacebookModel.IsPostAsSamePage)
                FacebookViewModel.FacebookModel.IsPostAsOwnAccount = true;
        }
    }
}
