using System.Windows.Input;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.Models;
using Prism.Commands;
using DominatorHouseCore.Utility;
using System.Linq;
using System;
using DominatorHouseCore;
using DominatorUIUtility.Views.AccountSetting.CustomControl;
using CommonServiceLocator;
using ProtoBuf;
using DominatorHouseCore.Enums;
using System.Runtime.InteropServices;

namespace DominatorUIUtility.ViewModel.Startup
{
    [ProtoContract]
    public class StartupBaseViewModel : Prism.Mvvm.BindableBase, IStartUpSearchQuery, IStartupJobConfiguration
    {
        [ProtoIgnore]
        [field:NonSerialized]
        public IRegionManager regionManager;

        [ProtoIgnore]
        [field: NonSerialized]
        public static int selectedIndex = 0;
        [ProtoIgnore]
     
        public static List<string> NavigationList { get; set; }
        [ProtoIgnore]
      
        public static List<ActivityConfig> ViewModelToSave { get; set; } = new List<ActivityConfig>();
        public StartupBaseViewModel(IRegionManager region)
        {
            regionManager = region;
            AddQueryCommand = new DelegateCommand<ActivitySetting>(AddQuery);
        }
        #region Commands
        [field: NonSerialized]
        public ICommand NextCommand { get; set; }
        [field: NonSerialized]
        public ICommand PreviousCommand { get; set; }
        [field: NonSerialized]
        public ICommand LoadedCommand { get; set; }
        [field: NonSerialized]
        public ICommand AddQueryCommand { get; set; }
        #endregion

        protected void NevigateNext()
        {
            if (selectedIndex >= NavigationList.Count - 1)
                return;
            selectedIndex++;
            var next = NavigationList[selectedIndex];
            regionManager.RequestNavigate("StartupRegion", next);
        }

        protected void NevigatePrevious()
        {
            if (selectedIndex <= 0)
                return;
            selectedIndex--;
            var previous = NavigationList[selectedIndex];
            regionManager.RequestNavigate("StartupRegion", previous);
        }
        public void OnLoad(string activityType)
        {
            ListQueryType.Clear();
            var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
            ListQueryType = NetworkFactory.GetNetworkfactory(viewModel.SelectedNetwork).GetActivity(activityType).GetQueryType();
            if (selectedIndex == NavigationList.Count - 1)
                NextButtonContent = "LangKeyFinish".FromResourceDictionary();
            else
                NextButtonContent = "LangKeyNext".FromResourceDictionary();
        }

        private JobConfiguration _jobConfiguration;
        [ProtoMember(1)]
        public JobConfiguration JobConfiguration
        {
            get
            {
                return _jobConfiguration;
            }
            set
            {
                if (value == _jobConfiguration)
                    return;
                SetProperty(ref _jobConfiguration, value);
            }
        }


        private ObservableCollection<QueryInfo> _savedQueries = new ObservableCollection<QueryInfo>();
        [ProtoMember(2)]
        public ObservableCollection<QueryInfo> SavedQueries
        {
            get
            {
                return _savedQueries;
            }
            set
            {
                SetProperty(ref _savedQueries, value);
            }
        }

        private List<string> _listQueryType = new List<string>();
        public List<string> ListQueryType
        {
            get
            {
                return _listQueryType;
            }
            set
            {
                SetProperty(ref _listQueryType, value);
            }
        }

        private string _nextButtonContent = "LangKeyNext".FromResourceDictionary();
        [ProtoIgnore]
        public string NextButtonContent
        {
            get { return _nextButtonContent; }
            set { SetProperty(ref _nextButtonContent, value); }
        }

        private void AddQuery(ActivitySetting actvity)
        {
            try
            {
                if (string.IsNullOrEmpty(actvity.QueryControl.CurrentQuery.QueryValue) && actvity.QueryControl.QueryCollection.Count != 0)
                {
                    actvity.QueryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = actvity.QueryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;

                        currentQuery.QueryValue = query;
                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                        currentQuery.QueryPriority = SavedQueries.Count + 1;

                        if (SavedQueries.Any(x => x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                        {
                            Dialog.ShowDialog("Warning", "Query already exist.");
                            return;
                        }
                        SavedQueries.Add(currentQuery);
                    });
                }
                else
                {
                    actvity.QueryControl.CurrentQuery.QueryTypeDisplayName = actvity.QueryControl.CurrentQuery.QueryType;
                    var currentQuery = actvity.QueryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryPriority = SavedQueries.Count + 1;
                    if (SavedQueries.Any(x => x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                    {
                        Dialog.ShowDialog("Warning", "Query already exist.");
                        return;
                    }
                    currentQuery.Index = SavedQueries.Count + 1;
                    SavedQueries.Add(currentQuery);
                    actvity.QueryControl.CurrentQuery = new QueryInfo();

                }
                actvity.QueryControl.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
    [ProtoContract]
    public class ActivityConfig
    {
        [ProtoMember(1)]
        public object Model { get; set; }
        [ProtoMember(2)]
        public ActivityType ActivityType { get; set; }
    }
  
}
