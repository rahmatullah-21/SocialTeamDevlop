using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.AccountSetting.CustomControl;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    
    public interface ILikeCommentViewModel
    {
    }
    public class LikeCommentViewModel : StartupBaseViewModel, ILikeCommentViewModel
    {

        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMulipleCommand { get; set; }

        public LikeCommentViewModel(IRegionManager region) : base(region)
        {

            DeleteQueryCommand = new DelegateCommand<object>(DeleteQuery);
            DeleteMulipleCommand = new DelegateCommand<object>(DeleteMuliple);

            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.LikeComment });

            NextCommand = new DelegateCommand(LikeCommentValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        private void LikeCommentValidate()
        {
            if (!IsActionasPageChecked && !IsActionasOwnAccountChecked)
            {
                Dialog.ShowDialog("Warning", "Please Select Reaction Type");
                return;
            }

            if (IsActionasPageChecked && ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please Select PageUrls");
                return;
            }
            if (SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one query.");
                return;
            }

            NavigateNext();
        }

        public bool _isActionasPageChecked;
        public bool IsActionasPageChecked
        {
            get { return _isActionasPageChecked; }
            set
            {
                if (value == _isActionasPageChecked)
                    return;
                SetProperty(ref _isActionasPageChecked, value);
            }
        }

        public bool _isActionasOwnAccountChecked = true;
        public bool IsActionasOwnAccountChecked
        {
            get { return _isActionasOwnAccountChecked; }
            set
            {
                if (value == _isActionasOwnAccountChecked)
                    return;
                SetProperty(ref _isActionasOwnAccountChecked, value);
            }
        }

        public string OwnPageUrl { get; set; }
        public List<string> ListOwnPageUrl { get; set; } = new List<string>();

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    SavedQueries.Remove(currentQuery);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void DeleteMuliple(object sender)
        {
            var selectedQuery = SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                {
                    try
                    {

                        if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            SavedQueries.Remove(currentQuery);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

    }
}
