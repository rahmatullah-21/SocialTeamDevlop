using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Models.SocioPublisher;
using System.Windows;
using LegionUIUtility.CustomControl;
using DominatorHouseCore.Enums.FdQuery;
using System.Collections.Generic;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore;

namespace LegionUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface IPageInviterViewModel
    {
        bool IsSelctDetails { get; set; }
        InviterDetails InviterDetailsModel { get; set; }
        InviterOptions InviterOptionsModel { get; set; }
        SelectAccountDetailsModel SelectAccountDetailsModel { get; set; }
    }
    public class PageInviterViewModel : StartupBaseViewModel, IPageInviterViewModel
    {
        public PageInviterViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.PageInviter });

            IsNonQuery = true;
            NextCommand = new DelegateCommand(PageInviterValidate);
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

        private void PageInviterValidate()
        {
            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if (InviterDetailsModel.IsProfileUrl && (selectAccountDetailsControl.GetPageInviterDetails
               (SelectAccountDetailsModel)).PageInviterDetails.Count == 0)
            {
                Dialog.ShowDialog( "Error", "Please select atleast one inviter details.");
                return;
            }
            if (!InviterDetailsModel.IsPostUrl && !InviterDetailsModel.IsRandomPosts 
                                               && !InviterDetailsModel.IsSpecificPosts)
            {
                Dialog.ShowDialog("Error", "Please select atleast one post option.");
                return;
            }
            if (InviterDetailsModel.IsSpecificPosts && InviterDetailsModel.ListPostUrl.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please select atleast one Specific post.");
                return;
            }

            NavigateNext();
        }

        private InviterOptions _inviterOptionsModel = new InviterOptions();
        public InviterOptions InviterOptionsModel
        {
            get { return _inviterOptionsModel; }
            set
            {
                if (_inviterOptionsModel == value & _inviterOptionsModel == null)
                    return;
                SetProperty(ref _inviterOptionsModel, value);
            }
        }

        private InviterDetails _inviterDetails = new InviterDetails();
        public InviterDetails InviterDetailsModel
        {
            get { return _inviterDetails; }
            set
            {
                if (_inviterDetails == value & _inviterDetails == null)
                    return;
                SetProperty(ref _inviterDetails, value);
            }
        }

        private SelectAccountDetailsModel _selectAccountDetailsModel=new SelectAccountDetailsModel();
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

    }
}
