using System;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.NetworkActivitySetting;
using Prism.Regions;
using Prism.Commands;
using System.Linq;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface ISelectActivityViewModel
    {
        void SetActivityTypeByNetwork(string network);
        SelectActivityModel SelectActivityModel { get; set; }
        DominatorAccountModel SelectAccount { get; set; }
        string SelectedNetwork { get; set; }
    }
    public class SelectActivityViewModel : StartupBaseViewModel, ISelectActivityViewModel
    {
        public SelectActivityViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(OnNextClick);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
        }

        private SelectActivityModel _selectActivityModel = new SelectActivityModel();

        public SelectActivityModel SelectActivityModel
        {
            get { return _selectActivityModel; }
            set { SetProperty(ref _selectActivityModel, value); }
        }
        private string _selectedNetwork;

        public string SelectedNetwork
        {
            get { return _selectedNetwork; }
            set
            {
                SetProperty(ref _selectedNetwork, value);
                if (SelectedNetwork != SocialNetworks.Admin.ToString())
                    SetActivityTypeByNetwork(SelectedNetwork);
            }
        }
        DominatorAccountModel _selectAccount = new DominatorAccountModel();
        public DominatorAccountModel SelectAccount
        {
            get { return _selectAccount; }
            set { SetProperty(ref _selectAccount, value); }
        }

        private void OnNextClick()
        {
            var allSelectedActivity = SelectActivityModel.LstNetworkActivityType.Where(x => x.IsActivity).ToList();
            if (allSelectedActivity.Count() == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(), "LangKeySelectAtleastOneActivity".FromResourceDictionary());
                return;
            }
            NavigationList = new List<string>();
            LstGlobalQuery = new Dictionary<Type, List<QueryInfo>>();
            NavigationList.Add("SelectActivity");
            allSelectedActivity.ForEach(name => NavigationList.Add(name.ActivityType));
            SocialNetworkActivity.RegisterNetwork();
            NavigateNext();
        }
        public void SetActivityTypeByNetwork(string network)
        {
            SelectActivityModel.LstNetworkActivityType.Clear();

            foreach (var name in Enum.GetNames(typeof(ActivityType)))
            {
                if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name)).Contains(network))
                {
                    SelectActivityModel.LstNetworkActivityType.Add(new ActivityChecked
                    {
                        ActivityType = name
                    });
                }
            }
        }
    }
}
