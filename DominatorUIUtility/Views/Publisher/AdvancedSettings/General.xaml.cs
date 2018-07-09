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
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class General : UserControl,INotifyPropertyChanged
    {
        private General()
        {
            InitializeComponent();
            MainGrid.DataContext = GeneralViewModel;
        }

        static General ObjGeneral = null;
        public static General GetSingeltonGeneralObject()
        {
            if (ObjGeneral == null)
                ObjGeneral = new General();
            return ObjGeneral;
        }
       
        private GeneralViewModel _generalViewModel = new GeneralViewModel();

        public GeneralViewModel GeneralViewModel
        {
            get
            {
                return _generalViewModel;
            }
            set
            {
                _generalViewModel = value;
                OnPropertyChanged(nameof(GeneralViewModel));
            }
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void General_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var generaldata = GenericFileManager.GetModuleDetails<GeneralModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            GeneralViewModel.GeneralModel = generaldata ?? (generaldata = new GeneralModel());
        }
    }
}
