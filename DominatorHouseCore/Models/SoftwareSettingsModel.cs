using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class SoftwareSettingsModel : BindableBase
    {
        private bool _isRunDHAtStartUpChecked;
        [ProtoMember(1)]
        public bool IsRunDHAtStartUpChecked
        {
            get
            {
                return _isRunDHAtStartUpChecked;
            }
            set
            {
                if (value == _isRunDHAtStartUpChecked)
                    return;
                SetProperty(ref _isRunDHAtStartUpChecked, value);
            }
        }
        private bool _isShowToolTipToSystemTrayChecked;
        [ProtoMember(2)]
        public bool IsShowToolTipToSystemTrayChecked
        {
            get
            {
                return _isShowToolTipToSystemTrayChecked;
            }
            set
            {
                if (value == _isShowToolTipToSystemTrayChecked)
                    return;
                SetProperty(ref _isShowToolTipToSystemTrayChecked, value);
            }
        }
        private bool _isRecreateDesktopshortcutAtStartupChecked;
        [ProtoMember(3)]
        public bool IsRecreateDesktopshortcutAtStartupChecked
        {
            get
            {
                return _isRecreateDesktopshortcutAtStartupChecked;
            }
            set
            {
                if (value == _isRecreateDesktopshortcutAtStartupChecked)
                    return;
                SetProperty(ref _isRecreateDesktopshortcutAtStartupChecked, value);
            }
        }
        private bool _isStartAppMinimizedChecked;
        [ProtoMember(4)]
        public bool IsStartAppMinimizedChecked
        {
            get
            {
                return _isStartAppMinimizedChecked;
            }
            set
            {
                if (value == _isStartAppMinimizedChecked)
                    return;
                SetProperty(ref _isStartAppMinimizedChecked, value);
            }
        }
        private bool _isShowSameAccountsChecked;
        [ProtoMember(5)]
        public bool IsShowSameAccountsChecked
        {
            get
            {
                return _isShowSameAccountsChecked;
            }
            set
            {
                if (value == _isShowSameAccountsChecked)
                    return;
                SetProperty(ref _isShowSameAccountsChecked, value);
            }
        }
        private bool _isEnableGlobalNightModeChecked;
        [ProtoMember(6)]
        public bool IsEnableGlobalNightModeChecked
        {
            get
            {
                return _isEnableGlobalNightModeChecked;
            }
            set
            {
                if (value == _isEnableGlobalNightModeChecked)
                    return;
                SetProperty(ref _isEnableGlobalNightModeChecked, value);
            }
        }
        private RangeUtilities _executeAnyActionsBetween = new RangeUtilities();
        [ProtoMember(7)]
        public RangeUtilities ExecuteAnyActionsBetween
        {
            get
            {
                return _executeAnyActionsBetween;
            }
            set
            {
                if (value == _executeAnyActionsBetween)
                    return;
                SetProperty(ref _executeAnyActionsBetween, value);
            }
        }
        private bool _isRandomiseNightModeChecked;
        [ProtoMember(8)]
        public bool IsRandomiseNightModeChecked
        {
            get
            {
                return _isRandomiseNightModeChecked;
            }
            set
            {
                if (value == _isRandomiseNightModeChecked)
                    return;
                SetProperty(ref _isRandomiseNightModeChecked, value);
            }
        }
        private RangeUtilities _startBetween = new RangeUtilities();
        [ProtoMember(9)]
        public RangeUtilities StartBetween
        {
            get
            {
                return _startBetween;
            }
            set
            {
                if (value == _startBetween)
                    return;
                SetProperty(ref _startBetween, value);
            }
        }
        private RangeUtilities _endBetween = new RangeUtilities();
        [ProtoMember(10)]
        public RangeUtilities EndBetween
        {
            get
            {
                return _endBetween;
            }
            set
            {
                if (value == _endBetween)
                    return;
                SetProperty(ref _endBetween, value);
            }
        }
        private bool _isIgnoreGlobalChecked;
        [ProtoMember(11)]
        public bool IsIgnoreGlobalChecked
        {
            get
            {
                return _isIgnoreGlobalChecked;
            }
            set
            {
                if (value == _isIgnoreGlobalChecked)
                    return;
                SetProperty(ref _isIgnoreGlobalChecked, value);
            }
        }
        private bool _isEnableGlobalFindReplaceChecked;
        [ProtoMember(12)]
        public bool IsEnableGlobalFindReplaceChecked
        {
            get
            {
                return _isEnableGlobalFindReplaceChecked;
            }
            set
            {
                if (value == _isEnableGlobalFindReplaceChecked)
                    return;
                SetProperty(ref _isEnableGlobalFindReplaceChecked, value);
            }
        }
        private int _waitMinimumOf;
        [ProtoMember(13)]
        public int WaitMinimumOf
        {
            get
            {
                return _waitMinimumOf;
            }
            set
            {
                if (value == _waitMinimumOf)
                    return;
                SetProperty(ref _waitMinimumOf, value);
            }
        }
        private bool _isUseTabInsteadOfCommaChecked;
        [ProtoMember(14)]
        public bool IsUseTabInsteadOfCommaChecked
        {
            get
            {
                return _isUseTabInsteadOfCommaChecked;
            }
            set
            {
                if (value == _isUseTabInsteadOfCommaChecked)
                    return;
                SetProperty(ref _isUseTabInsteadOfCommaChecked, value);
            }
        }

        private bool _isEnableContactDifferentUserChecked;
        [ProtoMember(15)]
        public bool IsEnableContactDifferentUserChecked
        {
            get
            {
                return _isEnableContactDifferentUserChecked;
            }
            set
            {
                if (value == _isEnableContactDifferentUserChecked)
                    return;
                SetProperty(ref _isEnableContactDifferentUserChecked, value);
            }
        }
        private bool _isWhenPostingVideosSelectRandomFrameChecked;
        [ProtoMember(16)]
        public bool IsWhenPostingVideosSelectRandomFrameChecked
        {
            get
            {
                return _isWhenPostingVideosSelectRandomFrameChecked;
            }
            set
            {
                if (value == _isWhenPostingVideosSelectRandomFrameChecked)
                    return;
                SetProperty(ref _isWhenPostingVideosSelectRandomFrameChecked, value);
            }
        }
        private bool _isChangeVideoHashBeforePostingChecked;
        [ProtoMember(17)]
        public bool IsChangeVideoHashBeforePostingChecked
        {
            get
            {
                return _isChangeVideoHashBeforePostingChecked;
            }
            set
            {
                if (value == _isChangeVideoHashBeforePostingChecked)
                    return;
                SetProperty(ref _isChangeVideoHashBeforePostingChecked, value);
            }
        }

        private bool _isDoNotShowInfoMessegesChecked;
        [ProtoMember(18)]
        public bool IsDoNotShowInfoMessegesChecked
        {
            get
            {
                return _isDoNotShowInfoMessegesChecked;
            }
            set
            {
                if (value == _isDoNotShowInfoMessegesChecked)
                    return;
                SetProperty(ref _isDoNotShowInfoMessegesChecked, value);
            }
        }
        private bool _isUseLocalPcCultureChecked;
        [ProtoMember(19)]
        public bool IsUseLocalPcCultureChecked
        {
            get
            {
                return _isUseLocalPcCultureChecked;
            }
            set
            {
                if (value == _isUseLocalPcCultureChecked)
                    return;
                SetProperty(ref _isUseLocalPcCultureChecked, value);
            }
        }

        private bool _isEnableAutoRestartSocinatorChecked;
        [ProtoMember(20)]
        public bool IsEnableAutoRestartSocinatorChecked
        {
            get
            {
                return _isEnableAutoRestartSocinatorChecked;
            }
            set
            {
                if (value == _isEnableAutoRestartSocinatorChecked)
                    return;
                SetProperty(ref _isEnableAutoRestartSocinatorChecked, value);
            }
        }
        private int _minHour;
        [ProtoMember(21)]
        public int MinHour
        {
            get
            {
                return _minHour;
            }
            set
            {
                if (value == _minHour)
                    return;
                SetProperty(ref _minHour, value);
            }
        }

        private RangeUtilities _availableUpdatesBetween = new RangeUtilities();
        [ProtoMember(22)]
        public RangeUtilities AvailableUpdatesBetween
        {
            get
            {
                return _availableUpdatesBetween;
            }
            set
            {
                if (value == _availableUpdatesBetween)
                    return;
                SetProperty(ref _availableUpdatesBetween, value);
            }
        }

        private int _limitNumberOfRecords;
        [ProtoMember(23)]
        public int LimitNumberOfRecords
        {
            get
            {
                return _limitNumberOfRecords;
            }
            set
            {
                if (value == _limitNumberOfRecords)
                    return;
                SetProperty(ref _limitNumberOfRecords, value);
            }
        }
        private int _limitNumberOfDeshboardSummary;
        [ProtoMember(24)]
        public int LimitNumberOfDeshboardSummary
        {
            get
            {
                return _limitNumberOfDeshboardSummary;
            }
            set
            {
                if (value == _limitNumberOfDeshboardSummary)
                    return;
                SetProperty(ref _limitNumberOfDeshboardSummary, value);
            }
        }
        private int _limitNumberOfPost;
        [ProtoMember(25)]
        public int LimitNumberOfPost
        {
            get
            {
                return _limitNumberOfPost;
            }
            set
            {
                if (value == _limitNumberOfPost)
                    return;
                SetProperty(ref _limitNumberOfPost, value);
            }
        }
        private bool _isRecreateDeshboardWindowChecked;
        [ProtoMember(26)]
        public bool IsRecreateDeshboardWindowChecked
        {
            get
            {
                return _isRecreateDeshboardWindowChecked;
            }
            set
            {
                if (value == _isRecreateDeshboardWindowChecked)
                    return;
                SetProperty(ref _isRecreateDeshboardWindowChecked, value);
            }
        }
        private bool _isForceCampaignsToKeepSameCreationDateChecked;
        [ProtoMember(27)]
        public bool IsForceCampaignsToKeepSameCreationDateChecked
        {
            get
            {
                return _isForceCampaignsToKeepSameCreationDateChecked;
            }
            set
            {
                if (value == _isForceCampaignsToKeepSameCreationDateChecked)
                    return;
                SetProperty(ref _isForceCampaignsToKeepSameCreationDateChecked, value);
            }
        }
        private bool _isEnablePerformanceAnalysisChecked;
        [ProtoMember(28)]
        public bool IsEnablePerformanceAnalysisChecked
        {
            get
            {
                return _isEnablePerformanceAnalysisChecked;
            }
            set
            {
                if (value == _isEnablePerformanceAnalysisChecked)
                    return;
                SetProperty(ref _isEnablePerformanceAnalysisChecked, value);
            }
        }
        private string embaddedBrowserUserAgent;
        [ProtoMember(29)]
        public string EmbaddedBrowserUserAgent
        {
            get
            {
                return embaddedBrowserUserAgent;
            }
            set
            {
                if (value == embaddedBrowserUserAgent)
                    return;
                SetProperty(ref embaddedBrowserUserAgent, value);
            }
        }
        private bool _isResetOpenFileDialogChecked;
        [ProtoMember(30)]
        public bool IsResetOpenFileDialogChecked
        {
            get
            {
                return _isResetOpenFileDialogChecked;
            }
            set
            {
                if (value == _isResetOpenFileDialogChecked)
                    return;
                SetProperty(ref _isResetOpenFileDialogChecked, value);
            }
        }
        private bool _isDisableSocialProfilesDescriptionTooltipChecked;
        [ProtoMember(31)]
        public bool IsDisableSocialProfilesDescriptionTooltipChecked
        {
            get
            {
                return _isDisableSocialProfilesDescriptionTooltipChecked;
            }
            set
            {
                if (value == _isDisableSocialProfilesDescriptionTooltipChecked)
                    return;
                SetProperty(ref _isDisableSocialProfilesDescriptionTooltipChecked, value);
            }
        }
        private bool _isFullChecked;
        [ProtoMember(32)]
        public bool IsFullChecked
        {
            get
            {
                return _isFullChecked;
            }
            set
            {
                if (value == _isFullChecked)
                    return;
                SetProperty(ref _isFullChecked, value);
            }
        }
        private bool _isNormalChecked;
        [ProtoMember(33)]
        public bool IsNormalChecked
        {
            get
            {
                return _isNormalChecked;
            }
            set
            {
                if (value == _isNormalChecked)
                    return;
                SetProperty(ref _isNormalChecked, value);
            }
        }
        private string _globalFindReplaceText;
        [ProtoMember(34)]
        public string GlobalFindReplaceText
        {
            get
            {
                return _globalFindReplaceText;
            }
            set
            {
                if (value == _globalFindReplaceText)
                    return;
                SetProperty(ref _globalFindReplaceText, value);
            }
        }

        [ProtoMember(35, IsRequired = false)]
        public bool IsEnableParallelActivitiesChecked
        {
            get { return _isEnableParallelActivitiesChecked; }
            set
            {
                if (value == _isEnableParallelActivitiesChecked)
                    return;
                SetProperty(ref _isEnableParallelActivitiesChecked, value);
            }
        }

        private int _accountSynchronizationHours = 24;
        [ProtoMember(36)]
        public int AccountSynchronizationHours
        {
            get
            {
                return _accountSynchronizationHours;
            }
            set
            {
                if (value == _accountSynchronizationHours)
                    return;
                SetProperty(ref _accountSynchronizationHours, value);
            }
        }

        private int _simultaneousAccountUpdate = 5;
        private bool _isEnableParallelActivitiesChecked;

        [ProtoMember(37)]
        public int SimultaneousAccountUpdateCount
        {
            get
            {
                return _simultaneousAccountUpdate;
            }
            set
            {
                if (value == _simultaneousAccountUpdate)
                    return;
                SetProperty(ref _simultaneousAccountUpdate, value);
            }
        }

        private bool _isEnableAdvancedUserMode = true;
        [ProtoMember(38)]
        public bool IsEnableAdvancedUserMode
        {
            get
            {
                return _isEnableAdvancedUserMode;
            }
            set
            {
                if (value == _isEnableAdvancedUserMode)
                    return;
                SetProperty(ref _isEnableAdvancedUserMode, value);
            }
        }
        private bool _isDoNotAutoLoginAccountsWhileAddingToSoftware;
        [ProtoMember(39)]
        public bool IsDoNotAutoLoginAccountsWhileAddingToSoftware
        {
            get
            {
                return _isDoNotAutoLoginAccountsWhileAddingToSoftware;
            }
            set
            {
                if (value == _isDoNotAutoLoginAccountsWhileAddingToSoftware)
                    return;
                SetProperty(ref _isDoNotAutoLoginAccountsWhileAddingToSoftware, value);
            }
        }
    }
}
