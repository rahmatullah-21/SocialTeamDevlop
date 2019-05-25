using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using DominatorHouseCore.Interfaces.StartUp;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommonServiceLocator;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ICommentViewModel : IStartupJobConfiguration, IStartUpSearchQuery
    {
    }
    public class CommentViewModel : StartupBaseViewModel, ICommentViewModel
    {
        public CommentViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxCommentPerDay".FromResourceDictionary(),
            };
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
        public void OnLoad(string activityType)
        {
            ListQueryType.Clear();
            var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
            ListQueryType = NetworkFactory.GetNetworkfactory(viewModel.SelectedNetwork).GetActivity(activityType).GetQueryType();
        }
    }
}
