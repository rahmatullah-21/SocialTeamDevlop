using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface ISelectNetworkViewModel
    {
        string SelectedNetwork { get; set; }
        ObservableCollection<string> LstNetwork { get; set; }
    }
    public class SelectNetworkViewModel : BindableBase, ISelectNetworkViewModel
    {
        public SelectNetworkViewModel()
        {
            InitilizeNetwork();
        }

        public ICommand BeginnerCommand { get; set; }

        private string _selectedNetwork;

        public string SelectedNetwork
        {
            get { return _selectedNetwork; }
            set { SetProperty(ref _selectedNetwork, value); }
        }
        private ObservableCollection<string> _lstNetwork = new ObservableCollection<string>();

        public ObservableCollection<string> LstNetwork
        {
            get { return _lstNetwork; }
            set { SetProperty(ref _lstNetwork, value); }
        }

        void InitilizeNetwork()
        {
            var networks = Enum.GetNames(typeof(SocialNetworks));
            networks.ForEach(network =>
            {
                LstNetwork.Add(network);
            });
        }
    }
}
