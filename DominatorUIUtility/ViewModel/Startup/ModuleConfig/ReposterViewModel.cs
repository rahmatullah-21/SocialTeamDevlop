using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
   
    public interface IReposterViewModel
    {
    }
    public class ReposterViewModel : StartupBaseViewModel, IReposterViewModel
    {
        public ReposterViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.Reposter });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfRepostsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfRepostsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfRepostsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfRepostsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxRepostPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}
