using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface ICancelSentRequestViewModel
    {
    }
    public class CancelSentRequestViewModel : StartupBaseViewModel, ICancelSentRequestViewModel
    {
        public CancelSentRequestViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.CancelSentRequest });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyCancelNumberOfRequestPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyCancelNumberOfRequestPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyCancelNumberOfRequestPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyCancelNumberOfMaximumRequestPerDay".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyCancelNumberOfRequestPerWeek".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
    }
}
