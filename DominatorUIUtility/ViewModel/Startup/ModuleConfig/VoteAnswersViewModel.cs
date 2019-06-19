using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
   
    public interface IVoteAnswersViewModel
    {
    }
    public class VoteAnswersViewModel : StartupBaseViewModel, IVoteAnswersViewModel
    {
        public VoteAnswersViewModel(IRegionManager region) : base(region)
        {
            //ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.VoteAnswers });

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
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }
    }
}
