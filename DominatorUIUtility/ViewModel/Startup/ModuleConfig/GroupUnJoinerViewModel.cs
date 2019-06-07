using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
   

    public interface IGroupUnJoinerViewModel : IFacebookModel, ILinkedInModel
    {
        bool IsCheckedBySoftware { get; set; }
        bool IsCheckedOutSideSoftware { get; set; }
        bool IsCheckedCustomGroupList { get; set; }
        string UrlInput { get; set; }
        List<string> UrlList { get; set; }
         ICommand SaveCommand { get; set; }
    }
    public class GroupUnJoinerViewModel : StartupBaseViewModel, IGroupUnJoinerViewModel
    {
        public Visibility FacebookElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility LinkedInElementsVisibility { get; set; } = Visibility.Collapsed;
        public ICommand SaveCommand { get; set; }

        #region linkedIn private properties
        private bool _IsCheckedBySoftware;
        private bool _IsCheckedOutSideSoftware;
        private bool _IsCheckedCustomGroupList;
        private string _UrlInput;
        private List<string> _UrlList;
        #endregion

        public GroupUnJoinerViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.GroupUnJoiner });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;
            ElementsVisibility.NetworkElementsVisibilty(this);
            SaveCommand = new BaseCommand<object>((sender) => true, Save);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfGroupUnjoinsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfGroupUnjoinsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfGroupUnjoinsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfGroupUnjoinsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxGroupUnjoinsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }

        #region linkedIn properties
        public bool IsCheckedBySoftware
        {
            get { return _IsCheckedBySoftware; }
            set { SetProperty(ref _IsCheckedBySoftware, value); }
        }

        public bool IsCheckedOutSideSoftware
        {
            get { return _IsCheckedOutSideSoftware; }
            set { SetProperty(ref _IsCheckedOutSideSoftware, value); }
        }

        public bool IsCheckedCustomGroupList
        {
            get { return _IsCheckedCustomGroupList; }
            set { SetProperty(ref _IsCheckedCustomGroupList, value); }
        }

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
        public void Save(object sender)
        {
            try
            {
                if (UrlInput.Contains("\r\n"))
                {
                    UrlList =
                        Regex.Split(UrlInput, "\r\n").ToList();
                    GlobusLogHelper.log.Info(UrlList.Count + "Group urls saved sucessfully");
                }
                else
                {
                    UrlList = new List<string>();
                    UrlList.Add(UrlInput.ToString());
                    GlobusLogHelper.log.Info("One Group url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        #endregion
    }
}
