using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IFollowBackViewModel
    {
    }
    public class FollowBackViewModel : StartupBaseViewModel, IFollowBackViewModel
    {
        public FollowBackViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.FollowBack });
            IsNonQuery = true;
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowbacksPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowbacksPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowbacksPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowbacksPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowbackPerDay".FromResourceDictionary(),

                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
        private bool _isFollowBack;

        public bool IsFollowBack
        {
            get { return _isFollowBack; }
            set { _isFollowBack = value; }
        }
        private bool _isAcceptFollowRequest;

        public bool IsAcceptFollowRequest
        {
            get { return _isAcceptFollowRequest; }
            set { _isAcceptFollowRequest = value; }
        }

    }
}
