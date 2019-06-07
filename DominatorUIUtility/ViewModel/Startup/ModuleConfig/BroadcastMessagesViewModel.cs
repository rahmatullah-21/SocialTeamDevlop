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
            AddQueryToMessageCommand = new DelegateCommand<object>( AddQueryToMessageControl);
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
        public ICommand AddMessagesCommand { get; set; }
        public ICommand AddQueryToMessageCommand { get; set; }
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
                var moduleSettingsUserControl = sender as ActivitySetting;
                if (!BroadcastMessagesModel.ManageMessagesModel.LstQueries.Any(x =>
                    moduleSettingsUserControl != null && (x.Content.QueryValue == moduleSettingsUserControl.QueryControl.CurrentQuery.QueryValue &&
                                                          x.Content.QueryType == moduleSettingsUserControl.QueryControl.CurrentQuery.QueryType)))
                {
                    if (moduleSettingsUserControl != null && moduleSettingsUserControl.QueryControl.CurrentQuery.QueryValue.Contains(","))
                        moduleSettingsUserControl.QueryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).Distinct().ForEach(query =>
                        {
                            var newquery = new QueryContent
                            {
                                Content = new QueryInfo
                                {
                                    QueryValue = query,
                                    QueryType = moduleSettingsUserControl.QueryControl.CurrentQuery.QueryType
                                }
                            };
                            BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(newquery);
                            BroadcastMessagesModel.LstDisplayManageMessageModel.ForEach(x =>
                            {
                                if (!x.LstQueries.Any(y =>
                                    newquery.Content.QueryType ==
                                    moduleSettingsUserControl.QueryControl.CurrentQuery.QueryType &&
                                    y.Content.QueryValue == newquery.Content.QueryValue))
                                    x.LstQueries.Add(newquery);
                            });
                        });
                    else if (moduleSettingsUserControl != null)
                    {
                        var newquery = new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = moduleSettingsUserControl.QueryControl.CurrentQuery.QueryValue,
                                QueryType = moduleSettingsUserControl.QueryControl.CurrentQuery.QueryType
                            }
                        };
                        BroadcastMessagesModel.ManageMessagesModel.LstQueries.Add(newquery);
                        BroadcastMessagesModel.LstDisplayManageMessageModel.ForEach(x =>
                        {
                            if (!x.LstQueries.Any(y =>
                                newquery.Content.QueryType ==
                                moduleSettingsUserControl.QueryControl.CurrentQuery.QueryType &&
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
    }
}
