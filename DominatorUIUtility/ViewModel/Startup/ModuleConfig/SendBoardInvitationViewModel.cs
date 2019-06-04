using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class BoardCollaboratorInfo : BindableBase
    {
        private string _boardUrl;

        public string BoardUrl
        {
            get
            {
                return _boardUrl;
            }
            set
            {
                if (_boardUrl != null && _boardUrl == value)
                    return;
                SetProperty(ref _boardUrl, value);
            }
        }

        private string _email;

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if (_email != null && _email == value)
                    return;
                SetProperty(ref _email, value);
            }
        }

        private string _account;

        public string Account
        {
            get
            {
                return _account;
            }
            set
            {
                if (_account != null && _account == value)
                    return;
                SetProperty(ref _account, value);
            }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex != 0 && _selectedIndex == value)
                    return;
                SetProperty(ref _selectedIndex, value);
            }
        }
    }
    public interface ISendBoardInvitationViewModel
    {
    }
    public class SendBoardInvitationViewModel : StartupBaseViewModel, ISendBoardInvitationViewModel
    {
        public SendBoardInvitationViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.SendBoardInvitation });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfSendBoardInvitationPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfSendBoardInvitationPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfSendBoardInvitationPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfSendBoardInvitationPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeySendBoardInvitationPerDay".FromResourceDictionary(),

                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }
        public BoardCollaboratorInfo CurrentBoardCollaborator = new BoardCollaboratorInfo();
        public ObservableCollectionBase<BoardCollaboratorInfo> ListBoardCollaboratorInfo = new ObservableCollectionBase<BoardCollaboratorInfo>();
    }
}
