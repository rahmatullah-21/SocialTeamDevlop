using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface ISelectNetworkViewModel
    {
        string SelectedNetwork { get; set; }
        ObservableCollection<SocialNetworks> LstNetwork { get; set; }
    }
    public class SelectNetworkViewModel : StartupBaseViewModel, ISelectNetworkViewModel
    {
        public SelectNetworkViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand<string>(OnNextClick);
            PreviousCommand = new DelegateCommand<string>(OnPreviousClick);
            InitilizeNetwork();
        }

        #region Properties
        private string _selectedNetwork;

        public string SelectedNetwork
        {
            get { return _selectedNetwork; }
            set
            {
                SetProperty(ref _selectedNetwork, value);
                CommonServiceLocator.ServiceLocator.Current.GetInstance<ISelectActivityViewModel>().SetActivityTypeByNetwork(SelectedNetwork);
            }
        }
        private ObservableCollection<SocialNetworks> _lstNetwork = new ObservableCollection<SocialNetworks>();

        public ObservableCollection<SocialNetworks> LstNetwork
        {
            get { return _lstNetwork; }
            set { SetProperty(ref _lstNetwork, value); }
        }
        #endregion
        private new void OnNextClick(string next)
        {
            if (!string.IsNullOrEmpty(SelectedNetwork?.ToString()))
                regionManager.RequestNavigate("StartupRegion", next);
            else
                Dialog.ShowDialog("Error", "Please Select a Network");
        }
       
        void InitilizeNetwork()
        {
            var networks = Enum.GetNames(typeof(SocialNetworks));
            networks.ForEach(network =>
            {
                var net = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), network);
                if (net != SocialNetworks.Social && net != SocialNetworks.Gplus)
                    LstNetwork.Add(net);
            });
        }
    }
}
