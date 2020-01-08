using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for Pinterest.xaml
    /// </summary>
    public partial class Pinterest : UserControl
    {
        private readonly IGenericFileManager _genericFileManager;
        public Pinterest()
        {
            _genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = PinterestViewModel;
        }
        static Pinterest ObjPinterest;
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
            //var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
            //    .PublisherCreateCampaignViewModel
            //    .PublisherCreateCampaignModel.CampaignId;
            //var pinterestModel = _genericFileManager.GetModuleDetails<PinterestModel>
            //        (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest))
            //    .FirstOrDefault(x => x.CampaignId == campaignId);
            //PinterestViewModel.PinterestModel = pinterestModel ?? (new PinterestModel());
        }
    }
}
