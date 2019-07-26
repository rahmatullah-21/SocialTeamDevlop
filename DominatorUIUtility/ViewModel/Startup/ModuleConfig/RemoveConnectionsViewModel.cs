using System;
using System.Collections.Generic;
using System.Windows.Input;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using DominatorHouseCore.Command;
using System.Text.RegularExpressions;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IRemoveConnectionsViewModel
    {
        bool IsCheckedBySoftware { get; set; }
        bool IsCheckedOutSideSoftware { get; set; }
        bool IsCheckedLangKeyCustomUserList { get; set; }
        bool IsCheckedConnectedBefore { get; set; }
        string UrlInput { get; set; }
        List<string> UrlList { get; set; }
        ICommand SaveCustomUserListCommand { get; set; }
        int Days { get; set; }
        int Hours { get; set; }
    }
    public class RemoveConnectionsViewModel : StartupBaseViewModel, IRemoveConnectionsViewModel
    {
        #region private modifiers
        private int _days;
        private int _hours;
        private bool _IsCheckedBySoftware;
        private bool _IsCheckedConnectedBefore;
        private bool _IsCheckedLangKeyCustomUserList;
        private bool _IsCheckedOutSideSoftware;
        private string _UrlInput;
        private List<string> _UrlList;

        #endregion
        public RemoveConnectionsViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.RemoveConnections });
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            SaveCustomUserListCommand = new BaseCommand<object>((sender) => true, SaveCustomUsers);
            IsNonQuery = true;
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfConnectionsToRemovePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfConnectionsToRemovePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfConnectionsToRemovePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfConnectionsToRemovePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionsToRemovePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        private void ValidateAndNevigate()
        {
            if (!IsCheckedBySoftware && !IsCheckedOutSideSoftware && !IsCheckedLangKeyCustomUserList)
            {
                Dialog.ShowDialog("Error", "select at least once of the connection sources");
                return;
            }
            if (IsCheckedLangKeyCustomUserList && string.IsNullOrEmpty(UrlInput))
            {
                Dialog.ShowDialog("Error", "Please enter user list.");
                return;
            }
            NavigateNext();
        }

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (UrlInput.Contains("\r\n"))
                {
                    UrlList = Regex.Split(UrlInput, "\r\n").ToList();
                    GlobusLogHelper.log.Info("" + UrlList.Count + " profile urls saved sucessfully");
                }
                else
                {
                    UrlList = new List<string>();
                    UrlList.Add(UrlInput);
                    GlobusLogHelper.log.Info("One profile url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public int Days
        {
            get { return _days; }
            set { SetProperty(ref _days, value); }
        }
        public int Hours
        {
            get { return _hours; }
            set { SetProperty(ref _hours, value); }
        }
        public bool IsCheckedBySoftware
        {
            get { return _IsCheckedBySoftware; }
            set { SetProperty(ref _IsCheckedBySoftware, value); }
        }

        public bool IsCheckedConnectedBefore
        {
            get { return _IsCheckedConnectedBefore; }
            set { SetProperty(ref _IsCheckedConnectedBefore, value); }
        }
        public bool IsCheckedLangKeyCustomUserList
        {
            get { return _IsCheckedLangKeyCustomUserList; }
            set { SetProperty(ref _IsCheckedLangKeyCustomUserList, value); }
        }

        public bool IsCheckedOutSideSoftware
        {
            get { return _IsCheckedOutSideSoftware; }
            set { SetProperty(ref _IsCheckedOutSideSoftware, value); }
        }

        public ICommand SaveCustomUserListCommand { get; set; }

        public string UrlInput
        {
            get { return _UrlInput; }
            set { SetProperty(ref _UrlInput, value); }
        }

        public List<string> UrlList
        {
            get { return _UrlList; }
            set { SetProperty(ref _UrlList, value); }
        }
    }
}
