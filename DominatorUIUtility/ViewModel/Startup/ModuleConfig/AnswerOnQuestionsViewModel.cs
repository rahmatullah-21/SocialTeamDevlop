using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IAnswerOnQuestionsViewModel
    {
    }
    public class AnswerOnQuestionsViewModel : StartupBaseViewModel, IAnswerOnQuestionsViewModel
    {
        public AnswerOnQuestionsViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.AnswerOnQuestions });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyAnswerScrapePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyAnswerScrapePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyAnswerScrapePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyAnswerScrapePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxAnswerScrapePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }
    }
}
