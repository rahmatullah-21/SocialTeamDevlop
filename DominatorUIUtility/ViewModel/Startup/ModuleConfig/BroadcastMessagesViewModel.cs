using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.AccountSetting.CustomControl;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class BroadcastMessagesModel : BindableBase
    {

        private ObservableCollection<ManageMessagesModel> _lstDisplayManageMessageModel = new ObservableCollection<ManageMessagesModel>();
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel
        {
            get
            {
                return _lstDisplayManageMessageModel;
            }
            set
            {
                if (_lstDisplayManageMessageModel != value)
                    SetProperty(ref _lstDisplayManageMessageModel, value);
            }
        }
        private ManageMessagesModel _manageMessagesModel = new ManageMessagesModel();
        public ManageMessagesModel ManageMessagesModel
        {
            get
            {
                return _manageMessagesModel;
            }
            set
            {
                SetProperty(ref _manageMessagesModel, value);
            }
        }

        private bool _isSpintaxChecked;

        public bool IsSpintaxChecked
        {
            get
            {
                return _isSpintaxChecked;
            }
            set
            {
                SetProperty(ref _isSpintaxChecked, value);
            }
        }
    }
    public interface IBroadcastMessagesViewModel
    {
    }
    public class BroadcastMessagesViewModel : StartupBaseViewModel, IBroadcastMessagesViewModel
    {
        public BroadcastMessagesViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.BroadcastMessages });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            AddMessagesCommand = new DelegateCommand<object>(AddMessages);
            AddQueryToMessageCommand = new DelegateCommand<object>(AddQueryToMessageControl);

            DeleteQueryCommand = new DelegateCommand<object>(DeleteQuery);
            DeleteMultipleCommand = new DelegateCommand(DeleteMultiple);
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

        #region Command
        public ICommand AddMessagesCommand { get; set; }
        public ICommand AddQueryToMessageCommand { get; set; }
        public ICommand DeleteQueryCommand { get; set; }
        public ICommand DeleteMultipleCommand { get; set; }
        #endregion

        private BroadcastMessagesModel _broadcastMessagesModel = new BroadcastMessagesModel();
        public BroadcastMessagesModel BroadcastMessagesModel
        {
            get
            {
                return _broadcastMessagesModel;
            }
            set
            {
                SetProperty(ref _broadcastMessagesModel, value);
            }
        }
        private void AddMessages(object sender)
        {
            try
            {
                var messageData = sender as MessagesControl;

                if (messageData == null) return;

                messageData.Messages.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

                if (messageData.Messages.SelectedQuery.Count == 0 || string.IsNullOrEmpty(messageData.Messages.MessagesText))
                {
                    Dialog.ShowDialog("Warning", "May be you didn't select any query or message is missing.");
                    return;
                }

                if (messageData.Messages.SelectedQuery.Count == 1 &&
                    messageData.Messages.SelectedQuery[0].Content.QueryType == "All")
                {
                    Dialog.ShowDialog("Warning", "May be you didn't select any query or message is missing.");
                    return;
                }
                messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

                BroadcastMessagesModel.LstDisplayManageMessageModel.Add(messageData.Messages);

                messageData.Messages = new ManageMessagesModel
                {
                    LstQueries = BroadcastMessagesModel.ManageMessagesModel.LstQueries
                };
                messageData.Messages.LstQueries.Select(x =>
                {
                    x.IsContentSelected = false;
                    return x;
                }).ToList();

                BroadcastMessagesModel.ManageMessagesModel = messageData.Messages;
                messageData.ComboBoxQueries.ItemsSource = BroadcastMessagesModel.ManageMessagesModel.LstQueries;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void AddQueryToMessageControl(object sender)
        {
            try
            {
                var activitySetting = sender as ActivitySettingWithoutButton;
                if (!BroadcastMessagesModel.ManageMessagesModel.LstQueries.Any(x =>
                    activitySetting != null && (x.Content.QueryValue == activitySetting.QueryControl.CurrentQuery.QueryValue &&
                                                          x.Content.QueryType == activitySetting.QueryControl.CurrentQuery.QueryType)))
                {
                    if (activitySetting != null && activitySetting.QueryControl.CurrentQuery.QueryValue.Contains(","))
                        activitySetting.QueryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ForEach(query =>
                        {
                            var newquery = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = query,
                                    QueryType = activitySetting.QueryControl.CurrentQuery.QueryType
                                }
                            };
                            BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(newquery);
                            BroadcastMessagesModel.LstDisplayManageMessageModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    newquery.Content.QueryType ==
                                    activitySetting.QueryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == newquery.Content.QueryValue))
                                    x.LstQueries.Add(newquery);
                            });
                        });
                    else if (activitySetting != null)
                    {
                        var newquery = new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = activitySetting.QueryControl.CurrentQuery.QueryValue,
                                QueryType = activitySetting.QueryControl.CurrentQuery.QueryType
                            }
                        };
                        BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(newquery);
                        BroadcastMessagesModel.LstDisplayManageMessageModel.ForEach(x =>
                        {
                            if (!x.LstQueries.Any(y =>
                                newquery.Content.QueryType ==
                                activitySetting.QueryControl.CurrentQuery.QueryType &&
                                y.Content.QueryValue == newquery.Content.QueryValue))
                                x.LstQueries.Add(newquery);
                        });
                    }

                }
                if (BroadcastMessagesModel.ManageMessagesModel.LstQueries[0].IsContentSelected)
                    BroadcastMessagesModel.ManageMessagesModel.LstQueries.Select(x =>
                    {
                        x.IsContentSelected = true;
                        return x;
                    }).ToList();
                AddQueryCommand.Execute(sender);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteQuery(object sender)
        {
            try
            {
                var currentQuery = sender as QueryInfo;

                var queryToDelete = BroadcastMessagesModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                        currentQuery != null && (x.Content.QueryValue == currentQuery.QueryValue
                                                 && x.Content.QueryType == currentQuery.QueryType));

                if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                    SavedQueries.Remove(currentQuery);

                BroadcastMessagesModel.ManageMessagesModel.LstQueries.Remove(queryToDelete);
                foreach (var message in BroadcastMessagesModel.LstDisplayManageMessageModel.ToList())
                {
                    var queryDelete = message.SelectedQuery.FirstOrDefault(x => currentQuery != null && (x.Content.QueryType == currentQuery.QueryType && x.Content.QueryValue == currentQuery.QueryValue));
                    message.SelectedQuery.Remove(queryDelete);

                    if (message.SelectedQuery.Count == 0)
                        BroadcastMessagesModel.LstDisplayManageMessageModel.Remove(message);
                }
                if (!BroadcastMessagesModel.ManageMessagesModel.LstQueries.Skip(1).Any())
                    BroadcastMessagesModel.ManageMessagesModel.LstQueries[0].IsContentSelected = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void DeleteMultiple()
        {
            var selectedQuery = SavedQueries.Where(x => x.IsQuerySelected).ToList();
            try
            {
                foreach (var currentQuery in selectedQuery)
                {
                    try
                    {
                        var queryToDelete = BroadcastMessagesModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                                x.Content.QueryValue == currentQuery.QueryValue
                                && x.Content.QueryType == currentQuery.QueryType);

                        if (SavedQueries.Any(x => currentQuery != null && x.Id == currentQuery.Id))
                            SavedQueries.Remove(currentQuery);

                        BroadcastMessagesModel.ManageMessagesModel.LstQueries.Remove(queryToDelete);
                        foreach (var message in BroadcastMessagesModel.LstDisplayManageMessageModel.ToList())
                        {
                            message.SelectedQuery.Remove(queryToDelete);
                            if (message.SelectedQuery.Count == 0)
                                BroadcastMessagesModel.LstDisplayManageMessageModel.Remove(message);
                        }
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
