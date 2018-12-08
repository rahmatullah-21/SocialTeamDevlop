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
    /// Interaction logic for Tumblr.xaml
    /// </summary>
    public partial class Tumblr : UserControl,INotifyPropertyChanged
    {
        public Tumblr()
        {
            InitializeComponent();
            MainGrid.DataContext = TumblrViewModel;
        }
        static Tumblr ObjTumblr;
        public static Tumblr GetSingeltonTumblr()
        {
            if (ObjTumblr == null)
                ObjTumblr = new Tumblr();
            return ObjTumblr;
        }
        private TumblrViewModel _tumblrViewModel = new TumblrViewModel();

        public TumblrViewModel TumblrViewModel
        {
            get
            {
                return _tumblrViewModel;
            }
            set
            {
                _tumblrViewModel = value;
                OnPropertyChanged(nameof(TumblrViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Tumblr_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var tumblrModel = GenericFileManager.GetModuleDetails<TumblrModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Tumblr))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            TumblrViewModel.TumblrModel = tumblrModel ?? (new TumblrModel());
        }
    }
}
