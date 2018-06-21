using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.ViewModel.AdvancedSettings;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for Instagram.xaml
    /// </summary>
    public partial class Instagram : UserControl,INotifyPropertyChanged
    {
        private Instagram()
        {
            InitializeComponent();
            MainGrid.DataContext = InstagramViewModel;
        }
        static Instagram ObjInstagram = null;
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
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var instagramModel = GenericFileManager.GetModuleDetails<InstagramModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            InstagramViewModel.InstagramModel = instagramModel ?? InstagramViewModel.InstagramModel;
        }
    }
}
