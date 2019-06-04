using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
 
    public interface IEventCreatorViewModel
    {
    }
    public class EventCreatorViewModel : StartupBaseViewModel, IEventCreatorViewModel
    {
        public EventCreatorViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.EventCreator });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyCreateNumberOfEventsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyCreateNumberOfEventsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyCreateNumberOfEventsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyCreateNumberOfMaximumEventsPerDay".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyCreateNumberOfEventsPerWeek".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
    }
}
