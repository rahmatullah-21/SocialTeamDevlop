using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ISendMessageToFollowerViewModel
    {
        bool IsCheckedSendMessageToNewFollowers { get; set; }
        bool IsChkMakeCaptionAsSpinText { get; set; }
        string TextMessage { get; set; }
    }
    public class SendMessageToFollowerViewModel : StartupBaseViewModel, ISendMessageToFollowerViewModel
    {
        public SendMessageToFollowerViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
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
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
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

        private bool _IsCheckedSendMessageToNewFollowers;
        public bool IsCheckedSendMessageToNewFollowers
        {
            get
            {
                return _IsCheckedSendMessageToNewFollowers;
            }

            set
            {
                if (_IsCheckedSendMessageToNewFollowers == value)
                    return;
                SetProperty(ref _IsCheckedSendMessageToNewFollowers, value);
            }
        }

        private bool _IsChkMakeCaptionAsSpinText;
        public bool IsChkMakeCaptionAsSpinText
        {
            get
            {
                return _IsChkMakeCaptionAsSpinText;
            }

            set
            {
                if (_IsChkMakeCaptionAsSpinText == value)
                    return;
                SetProperty(ref _IsChkMakeCaptionAsSpinText, value);
            }
        }


        private string _textMessage = string.Empty;
        public string TextMessage
        {
            get
            {
                return _textMessage;
            }
            set
            {
                if (_textMessage == value)
                    return;
                SetProperty(ref _textMessage, value);
            }
        }
    }
}
