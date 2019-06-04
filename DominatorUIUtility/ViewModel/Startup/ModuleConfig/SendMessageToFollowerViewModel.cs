using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ISendMessageToFollowerViewModel
    {
    }
    public class SendMessageToFollowerViewModel : StartupBaseViewModel, ISendMessageToFollowerViewModel
    {
        public SendMessageToFollowerViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.SendMessageToFollower });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfMessagesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfMessagesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfMessagesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfMessagesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxMessagesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message == value)
                    return;
                SetProperty(ref _message, value);

            }
        }
    }
}
