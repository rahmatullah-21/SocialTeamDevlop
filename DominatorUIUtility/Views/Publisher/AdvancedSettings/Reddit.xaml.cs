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
    /// Interaction logic for Reddit.xaml
    /// </summary>
    public partial class Reddit : UserControl
    {
        private readonly IGenericFileManager _genericFileManager;
        public Reddit()
        {
            _genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = RedditViewModel;
        }

        static Reddit _objReddit;

        public static Reddit GetSingeltonRedditObject()
            => _objReddit ?? (_objReddit = new Reddit());

        private RedditViewModel _redditViewModel = new RedditViewModel();

        public RedditViewModel RedditViewModel
        {
            get
            {
                return _redditViewModel;
            }
            set
            {
                _redditViewModel = value;
                OnPropertyChanged(nameof(RedditViewModel));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Reddit_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var redditModel = _genericFileManager.GetModuleDetails<RedditModel>
                (ServiceLocator.Current.GetInstance<IConstantVariable>().GetPublisherOtherConfigFile(SocialNetworks.Reddit))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            RedditViewModel.RedditModel = redditModel ?? (new RedditModel());
        }
    }
}
