using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class AutoReplyToNewMessageModel : BindableBase
    {
      

        private bool _isReplyToMessagesThatContainSpecificWord﻿Checked;
    
        public bool IsReplyToMessagesThatContainSpecificWord﻿Checked
        {
            get
            {
                return _isReplyToMessagesThatContainSpecificWord﻿Checked;
            }
            set
            {
                if (_isReplyToMessagesThatContainSpecificWord﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToMessagesThatContainSpecificWord﻿Checked, value);

            }
        }
        private bool _isReplyToPendingMessages﻿﻿Checked;
  
        public bool IsReplyToPendingMessages﻿﻿Checked
        {
            get
            {
                return _isReplyToPendingMessages﻿﻿Checked;
            }
            set
            {
                if (_isReplyToPendingMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToPendingMessages﻿﻿Checked, value);

            }
        }
        private bool _isReplyToAllMessages﻿﻿Checked;
      
        public bool IsReplyToAllMessagesChecked
        {
            get
            {
                return _isReplyToAllMessages﻿﻿Checked;
            }
            set
            {
                if (_isReplyToAllMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToAllMessages﻿﻿Checked, value);

            }
        }
        private string _specificWord;

        public string SpecificWord
        {
            get
            {
                return _specificWord;
            }
            set
            {
                if (_specificWord == value)
                    return;
                SetProperty(ref _specificWord, value);

            }
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

        private bool _isChkAutoReplayPrivateBlacklist;
 
        public bool IsChkAutoReplyPrivateBlacklist
        {
            get
            {
                return _isChkAutoReplayPrivateBlacklist;
            }
            set
            {
                if (_isChkAutoReplayPrivateBlacklist == value)
                    return;
                SetProperty(ref _isChkAutoReplayPrivateBlacklist, value);

            }
        }

        private bool _isChkAutoReplayGroupBlacklist;

        public bool IsChkAutoReplyGroupBlacklist
        {
            get
            {
                return _isChkAutoReplayGroupBlacklist;
            }
            set
            {
                if (_isChkAutoReplayGroupBlacklist == value)
                    return;
                SetProperty(ref _isChkAutoReplayGroupBlacklist, value);

            }
        }

        private List<string> _lstMultiMessageForUserHasNotReplied = new List<string>();
 
        public List<string> LstMultiMessageForUserHasNotReplied
        {
            get
            {
                return _lstMultiMessageForUserHasNotReplied;
            }

            set
            {
                if (_lstMultiMessageForUserHasNotReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasNotReplied, value);
            }
        }
        private List<string> _lstMultiMessageForUserHasReplied = new List<string>();
     
        public List<string> LstMultiMessageForUserHasReplied
        {
            get
            {
                return _lstMultiMessageForUserHasReplied;
            }

            set
            {
                if (_lstMultiMessageForUserHasReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasReplied, value);
            }
        }

       
    }
    public interface IAutoReplyToNewMessageViewModel
    {
    }
    public class AutoReplyToNewMessageViewModel : StartupBaseViewModel, IAutoReplyToNewMessageViewModel
    {
        public AutoReplyToNewMessageViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.AutoReplyToNewMessage });
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
        private AutoReplyToNewMessageModel _autoReplyToNewMessageModel = new AutoReplyToNewMessageModel();

        public AutoReplyToNewMessageModel AutoReplyToNewMessageModel
        {
            get
            {
                return _autoReplyToNewMessageModel;
            }
            set
            {
                if (_autoReplyToNewMessageModel == null & _autoReplyToNewMessageModel == value)
                    return;
                SetProperty(ref _autoReplyToNewMessageModel, value);
            }
        }
    }
}
