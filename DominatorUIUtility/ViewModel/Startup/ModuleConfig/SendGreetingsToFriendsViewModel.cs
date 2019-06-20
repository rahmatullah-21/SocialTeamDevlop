using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface ISendGreetingsToFriendsViewModel
    {
        ManageMessagesModel ManageMessagesModel { get; set; }
        bool IsMessageAsPreview { get; set; }
        bool IsSpintaxChecked { get; set; }
        bool IsTagChecked { get; set; }
    }
    public class SendGreetingsToFriendsViewModel : StartupBaseViewModel, ISendGreetingsToFriendsViewModel
    {
        public ICommand AddMessagesCommand { get; set; }
        public SendGreetingsToFriendsViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.SendGreetingsToFriends });
            NextCommand = new DelegateCommand(SendGreetingsToFriendsValidate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            AddMessagesCommand = new BaseCommand<object>((sender) => true, AddMessages);

            ManageMessagesModel.LstQueries.Add(new QueryContent { Content = new QueryInfo() { QueryType = "All", QueryValue = "All" } });
            ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo()
                {
                    QueryType = Application.Current.FindResource("LangKeyGreetingOptions")?.ToString(),
                    QueryValue = Application.Current.FindResource("LangKeyTodaysBirthdays")?.ToString()
                }
            });

            ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo()
                {
                    QueryType = Application.Current.FindResource("LangKeyGreetingOptions")?.ToString(),
                    QueryValue = Application.Current.FindResource("LangKeyUpcomingBirthdays")?.ToString()
                }
            });

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyMessagesToNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyMessagesToNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyMessagesToNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyMessageToNumberOfProfilesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMessagesToMaxProfilesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        private void SendGreetingsToFriendsValidate()
        {
            if (LstManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog("Error", "Please add at least one message.");
                return;
            }
            if (IsFilterByDays && DaysBefore.StartValue == 0 && DaysBefore.EndValue == 0)
            {
                Dialog.ShowDialog("Error", "Please select valid days filter.");
                return;
            }

            NavigateNext();
        }

        public ObservableCollection<ManageMessagesModel> _lstManageMessagesModel =
            new ObservableCollection<ManageMessagesModel>();
        public ObservableCollection<ManageMessagesModel> LstManageMessagesModel
        {
            get { return _lstManageMessagesModel; }
            set
            {
                if (_lstManageMessagesModel == null & _lstManageMessagesModel == value)
                    return;
            }
        }

        private ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();
        public ManageMessagesModel ManageMessagesModel
        {
            get { return _manageMessagesModel; }
            set
            {
                if (_manageMessagesModel == null & _manageMessagesModel == value)
                    return;
            }
        }

        private bool _isSendBirthDayGreetings = true;
        public bool IsSendBirthDayGreetings
        {
            get { return _isSendBirthDayGreetings; }

            set
            {
                if (value == _isSendBirthDayGreetings)
                    return;
                SetProperty(ref _isSendBirthDayGreetings, value);
            }
        }

        private bool _isMessageAsPreview;
        public bool IsMessageAsPreview
        {
            get { return _isMessageAsPreview; }
            set
            {
                if (value == _isMessageAsPreview)
                    return;
                SetProperty(ref _isMessageAsPreview, value);
            }
        }


        private bool _isSpintaxChecked;
        public bool IsSpintaxChecked
        {
            get { return _isSpintaxChecked; }
            set
            {
                if (value == _isSpintaxChecked)
                    return;
                SetProperty(ref _isSpintaxChecked, value);
            }
        }

        private bool _isTagChecked;
        public bool IsTagChecked
        {
            get { return _isTagChecked; }
            set
            {
                if (value == _isTagChecked)
                    return;
                SetProperty(ref _isTagChecked, value);
            }
        }

        private RangeUtilities _daysBefore = new RangeUtilities(1, 2);
        public RangeUtilities DaysBefore
        {
            get { return _daysBefore; }

            set
            {
                if (value == _daysBefore)
                    return;
                SetProperty(ref _daysBefore, value);
            }
        }


        private RangeUtilities _userAge = new RangeUtilities(20, 60);
        public RangeUtilities UserAge
        {
            get { return _userAge; }

            set
            {
                if (value == _userAge)
                    return;
                SetProperty(ref _userAge, value);
            }
        }

        private bool _isFilterByAge = true;
        public bool IsFilterByAge
        {
            get { return _isFilterByAge; }

            set
            {
                if (value == _isFilterByAge)
                    return;
                SetProperty(ref _isFilterByAge, value);
            }
        }


        private bool _isFilterByDays = true;
        public bool IsFilterByDays
        {
            get { return _isFilterByDays; }

            set
            {
                if (value == _isFilterByDays)
                    return;
                SetProperty(ref _isFilterByDays, value);
            }
        }

        private void AddMessages(object sender)
        {
            var messageData = sender as MessageMediaControl;
            if (messageData == null) return;
            messageData.Messages.SelectedQuery =
                new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Messages.SelectedQuery.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please select atleast one query!!");
                return;
            }

            if (string.IsNullOrEmpty(messageData.Messages.MessagesText))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please enter message text!!");
                return;
            }

            messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));
            LstManageMessagesModel.Add(messageData.Messages);
            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = ManageMessagesModel.LstQueries
            };

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            messageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();
            ManageMessagesModel = messageData.Messages;
            messageData.ComboBoxQueries.ItemsSource = ManageMessagesModel.LstQueries;

        }
    }
}
