using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using DominatorHouseCore.Models.SocioPublisher;

namespace LegionUIUtility.ViewModel.Startup.ModuleConfig
{
    
    public interface IGroupInviterViewModel
    {
        bool IsSelctDetails { get; set; }
        InviterOptions InviterOptionsModel { get; set; }
        SelectAccountDetailsModel SelectAccountDetailsModel { get; set; }
    }
    public class GroupInviterViewModel : StartupBaseViewModel, IGroupInviterViewModel
    {
        public GroupInviterViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.GroupInviter });

            IsNonQuery = true;
            NextCommand = new DelegateCommand(GroupInviterValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyInviteNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyInviteNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyInviteNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyInviteToNumberOfProfilesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyInviteMaxProfilesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        private void GroupInviterValidate()
        {

            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if ((selectAccountDetailsControl.GetGroupInviterDetails(SelectAccountDetailsModel)).GroupInviterDetails.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one inviter details.");
                return;
            }
            if (InviterOptionsModel.IsSendInvitationWithNote && string.IsNullOrEmpty(InviterOptionsModel.Note))
            {
                Dialog.ShowDialog("Error", "Please enter a note.");
                return;
            }
            
            NavigateNext();
        }

        private SelectAccountDetailsModel _selectAccountDetailsModel = new SelectAccountDetailsModel();
        public SelectAccountDetailsModel SelectAccountDetailsModel
        {
            get { return _selectAccountDetailsModel; }
            set
            {
                if (_selectAccountDetailsModel == value & _selectAccountDetailsModel == null)
                    return;
                SetProperty(ref _selectAccountDetailsModel, value);
            }

        }

        private bool _isSelctDetails;
        public bool IsSelctDetails
        {
            get { return _isSelctDetails; }
            set
            {
                if (_isSelctDetails == value)
                    return;
                SetProperty(ref _isSelctDetails, value);
            }
        }

        private InviterOptions _inviterOptions=new InviterOptions();
        public InviterOptions InviterOptionsModel
        {
            get { return _inviterOptions; }
            set
            {
                if (_inviterOptions == null & _inviterOptions == value)
                    return;
                SetProperty(ref _inviterOptions, value);
            }
        }

        private bool _isFilterMemberChkd;
        public bool IsFilterMemberChkd
        {
            get { return _isFilterMemberChkd; }
            set
            {
                if (_isFilterMemberChkd == value)
                    return;
                SetProperty(ref _isFilterMemberChkd, value);
            }
        }

    }
}
