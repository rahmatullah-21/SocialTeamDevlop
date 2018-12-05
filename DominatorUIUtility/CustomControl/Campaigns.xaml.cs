using DominatorHouseCore.Enums;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : UserControl, INotifyPropertyChanged
    {
        private CampaignViewModel _campaignViewModel = new CampaignViewModel();

        public CampaignViewModel CampaignViewModel
        {
            get
            {
                return _campaignViewModel;
            }
            set
            {
                _campaignViewModel = value;
                OnPropertyChanged(nameof(CampaignViewModel));
            }
        }

        public Campaigns(SocialNetworks socialNetworks)
        {
            InitializeComponent();
            CampaignViewModel.SocialNetworks = socialNetworks;
            CampaignViewModel.SetActivityTypes();
            Campaign.DataContext = CampaignViewModel;
            CampaignViewModel.CampaignCollection = CollectionViewSource.GetDefaultView(CampaignViewModel.LstCampaignDetails);
            instance = this;
        }

        private static Campaigns instance = null;

        public static Campaigns GetCampaignsInstance(SocialNetworks socialNetworks)
        {
            return instance ?? (instance = new Campaigns(socialNetworks));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
