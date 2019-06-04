using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    
    public interface IGroupJoinerViewModel
    {
    }
    public class GroupJoinerViewModel : StartupBaseViewModel, IGroupJoinerViewModel
    {
        public GroupJoinerViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.GroupJoiner });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName ="LangKeyNumberOfGroupJoinerRequestPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName ="LangKeyNumberOfGroupJoinerRequestPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName ="LangKeyNumberOfGroupJoinerRequestPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName ="LangKeyNumberOfGroupJoinerRequestPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName ="LangKeyMaxGroupJoinerRequestPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
    }
}
