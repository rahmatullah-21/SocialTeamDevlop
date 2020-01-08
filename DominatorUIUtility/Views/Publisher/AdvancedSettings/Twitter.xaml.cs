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
    /// Interaction logic for Twitter.xaml
    /// </summary>
    public partial class Twitter : UserControl, INotifyPropertyChanged
    {
        private readonly IGenericFileManager _genericFileManager;
        public Twitter()
        {
            _genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = TwitterViewModel;
        }
        static Twitter ObjTwitter;
        public static Twitter GetSingletonTwitterObject()
        {
            if (ObjTwitter == null)
                ObjTwitter = new Twitter();
            return ObjTwitter;
        }
        private TwitterViewModel _twitterViewModel = new TwitterViewModel();

        public TwitterViewModel TwitterViewModel
        {
            get
            {
                return _twitterViewModel;
            }
            set
            {
                _twitterViewModel = value;
                OnPropertyChanged(nameof(TwitterViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Twitter_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
            //    .PublisherCreateCampaignViewModel
            //    .PublisherCreateCampaignModel.CampaignId;
            //var twitterModel = _genericFileManager.GetModuleDetails<TwitterModel>
            //        (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Twitter))
            //    .FirstOrDefault(x => x.CampaignId == campaignId);
            //TwitterViewModel.TwitterModel = twitterModel ?? (new TwitterModel());
        }
    }
}
