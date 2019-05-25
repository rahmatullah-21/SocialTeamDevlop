using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using DominatorHouseCore.Interfaces.StartUp;
using System.Collections.ObjectModel;
using CommonServiceLocator;
using System.Collections.Generic;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IFollowViewModel : IStartupJobConfiguration,IStartUpSearchQuery
    {
    }
    public class FollowViewModel : StartupBaseViewModel, IFollowViewModel
    {
        public FollowViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowsPerDay".FromResourceDictionary(),
            };
            ListQueryType.Clear();
          

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
        private JobConfiguration _jobConfiguration;

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

        public void OnLoad(string activityType)
        {
            ListQueryType.Clear();
            var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
          ListQueryType = NetworkFactory.GetNetworkfactory(viewModel.SelectedNetwork).GetActivity(activityType).GetQueryType();
        }
    }

}
