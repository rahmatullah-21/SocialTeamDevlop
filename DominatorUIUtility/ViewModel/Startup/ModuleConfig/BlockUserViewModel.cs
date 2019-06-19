using System;
using System.Windows.Input;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using DominatorHouseCore.Command;
using System.Text.RegularExpressions;
using System.Linq;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IBlockUserViewModel
    {
        string UrlInput { get; set; }
        ICommand SaveCustomUserListCommand { get; set; }
        List<string> UrlList { get; set; }
    }
    public class BlockUserViewModel : StartupBaseViewModel, IBlockUserViewModel
    {
        public BlockUserViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.BlockUser });
            SaveCustomUserListCommand = new BaseCommand<object>((sender) => true, SaveCustomUsers);
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUsersToBlockPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUsersToBlockPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUsersToBlockPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUsersToBlockPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUsersToBlockPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }


        private string _UrlInput;
        private List<string> _urlList;

        public string UrlInput
        {
            get { return _UrlInput; }
            set { SetProperty(ref _UrlInput, value); }
        }

        public ICommand SaveCustomUserListCommand { get; set; }

        public List<string> UrlList
        {
            get { return _urlList; }
            set { SetProperty(ref _urlList, value); }
        }

        private void SaveCustomUsers(object sender)
        {
            try
            {
                if (UrlInput.Contains("\r\n"))
                {
                    UrlList =
                        Regex.Split(UrlInput, "\r\n").Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ToList();
                    GlobusLogHelper.log.Info("" + UrlList.Count + " profile urls saved sucessfully");
                }
                else
                {
                    UrlList = new List<string> { UrlInput };
                    GlobusLogHelper.log.Info("One profile url saved sucessfully");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
