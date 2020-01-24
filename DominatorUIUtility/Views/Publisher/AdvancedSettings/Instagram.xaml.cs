using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.AdvancedSettings;
using LegionUIUtility.Views.SocioPublisher;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace LegionUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for Instagram.xaml
    /// </summary>
    public partial class Instagram : UserControl, INotifyPropertyChanged
    {
        private readonly IGenericFileManager _genericFileManager;
        public Instagram()
        {
            _genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = InstagramViewModel;
        }
        static Instagram ObjInstagram;
        public static Instagram GetSingeltonInstagramObject()
        {
            if (ObjInstagram == null)
                ObjInstagram = new Instagram();
            return ObjInstagram;
        }
        private InstagramViewModel _instagramViewModel = new InstagramViewModel();

        public InstagramViewModel InstagramViewModel
        {
            get
            {
                return _instagramViewModel;
            }
            set
            {
                _instagramViewModel = value;
                OnPropertyChanged(nameof(InstagramViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void Instagram_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
            //    .PublisherCreateCampaignViewModel
            //    .PublisherCreateCampaignModel.CampaignId;
            //var instagramModel = _genericFileManager.GetModuleDetails<InstagramModel>
            //        (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram))
            //    .FirstOrDefault(x => x.CampaignId == campaignId);
            //InstagramViewModel.InstagramModel = instagramModel ?? (new InstagramModel());
        }
    }
}
