using CommonServiceLocator;
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

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for GooglePlus.xaml
    /// </summary>
    public partial class GooglePlus : UserControl, INotifyPropertyChanged
    {
        private readonly IGenericFileManager _genericFileManager;
        public GooglePlus()
        {
            _genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = GooglePlusViewModel;
        }
        static GooglePlus ObJGooglePlus;
        public static GooglePlus GetSingeltonGooglePlusObject()
        {
            if (ObJGooglePlus == null)
                ObJGooglePlus = new GooglePlus();
            return ObJGooglePlus;
        }
        private GooglePlusViewModel _googlePlusViewModel = new GooglePlusViewModel();

        public GooglePlusViewModel GooglePlusViewModel
        {
            get
            {
                return _googlePlusViewModel;
            }
            set
            {
                _googlePlusViewModel = value;
                OnPropertyChanged(nameof(GooglePlusViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GooglePlus_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var googlePlusModel = _genericFileManager.GetModuleDetails<GooglePlusModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Gplus))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            GooglePlusViewModel.GooglePlusModel = googlePlusModel ?? (new GooglePlusModel());
        }
    }
}
