using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface IAutoReplyToNewMessageViewModel
    {
    }
    public class AutoReplyToNewMessageViewModel : StartupBaseViewModel, IAutoReplyToNewMessageViewModel
    {
        public AutoReplyToNewMessageViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.AutoReplyToNewMessage });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            AddMessagesCommand = new DelegateCommand<object>(AddMessages);
            InputSaveCommand = new DelegateCommand<object>(SaveInput);
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
        public ICommand InputSaveCommand { get; set; }
        private List<string> _lstMessage=new List<string>();
        public List<string> LstMessage
        {
            get
            {
                return _lstMessage;
            }
            set
            {
                if (_lstMessage == value)
                    return;
                SetProperty(ref _lstMessage, value);
            }
        }
        private void SaveInput(object sender)
        {
            try
            {
                List<string> lstSpecificWords = Regex.Split(SpecificWord, "\n").Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).ToList();
                LstMessage = lstSpecificWords;

                int count = ManageMessagesModel.LstQueries.Count;

                while (count > 1)
                {
                    var Content = ManageMessagesModel.LstQueries[count - 1].Content;

                    if (Content.QueryValue != "All")
                    {
                        ManageMessagesModel.LstQueries.RemoveAt(count - 1);
                    }
                    count--;
                }

                lstSpecificWords.ForEach(x =>
                {
                    if (ManageMessagesModel.LstQueries.All(y => y.Content.QueryValue != x))
                    {
                        ManageMessagesModel.LstQueries.Add(new QueryContent
                        {
                            Content = new QueryInfo
                            {
                                QueryValue = x
                            }

                        });
                    }
                });

                GlobusLogHelper.log.Info($"{lstSpecificWords.Count} specific word{(lstSpecificWords.Count > 1 ? "s" : "")} saved and added to query sucessfully!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void AddMessages(object sender)
        {
            var messageData = sender as MessageMediaControl;

            if (messageData?.Messages.MessagesText == null) return;

            messageData.Messages.SelectedQuery = new ObservableCollection<QueryContent>(messageData.Messages.LstQueries.Where(x => x.IsContentSelected));

            if (messageData.Messages.SelectedQuery.Count == 0)
            {
                GlobusLogHelper.log.Info("Please add query type with message(s)");
                return;
            }
          
            messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            if (messageData.Messages.MessagesText != null)
            {
                List<string> listMessages = messageData.Messages.MessagesText.Split('\n').ToList();
                listMessages = listMessages.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(y => y.Trim()).ToList();

                listMessages.ForEach(message =>
                {
                    try
                    {
                        bool isContain = false;
                        LstDisplayManageMessageModel.ForEach(lstMessage =>
                        {
                            if (lstMessage.MessagesText.ToLower().Equals(message.ToLower()))
                                isContain = lstMessage.SelectedQuery.Any(x => messageData.Messages.SelectedQuery.Contains(x));
                        });
                        if (!isContain)
                            LstDisplayManageMessageModel.Add(new ManageMessagesModel() { MessagesText = message, SelectedQuery = messageData.Messages.SelectedQuery, MessageId = messageData.Messages.MessageId, LstQueries = messageData.Messages.LstQueries });
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    //AddToList(messageData.Messages, message);
                });
            }
            else
                LstDisplayManageMessageModel.Add(messageData.Messages);

            messageData.Messages = new ManageMessagesModel
            {
                LstQueries = ManageMessagesModel.LstQueries
            };

            messageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

            ManageMessagesModel = messageData.Messages;

            messageData.ComboBoxQueries.ItemsSource = ManageMessagesModel.LstQueries;

        }
        private bool _isReplyToMessagesThatContainSpecificWord﻿Checked;

        public bool IsReplyToMessagesThatContainSpecificWord﻿Checked
        {
            get
            {
                return _isReplyToMessagesThatContainSpecificWord﻿Checked;
            }
            set
            {
                if (_isReplyToMessagesThatContainSpecificWord﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToMessagesThatContainSpecificWord﻿Checked, value);

            }
        }
        private bool _isReplyToPendingMessages﻿﻿Checked;

        public bool IsReplyToPendingMessages﻿﻿Checked
        {
            get
            {
                return _isReplyToPendingMessages﻿﻿Checked;
            }
            set
            {
                if (_isReplyToPendingMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToPendingMessages﻿﻿Checked, value);

            }
        }
        private bool _isReplyToAllMessages﻿﻿Checked;

        public bool IsReplyToAllMessagesChecked
        {
            get
            {
                return _isReplyToAllMessages﻿﻿Checked;
            }
            set
            {
                if (_isReplyToAllMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToAllMessages﻿﻿Checked, value);

            }
        }
        private string _specificWord;

        public string SpecificWord
        {
            get
            {
                return _specificWord;
            }
            set
            {
                if (_specificWord == value)
                    return;
                SetProperty(ref _specificWord, value);

            }
        }

        private bool _IsChkMakeCaptionAsSpinText;
      
        public bool IsChkMakeCaptionAsSpinText
        {
            get
            {
                return _IsChkMakeCaptionAsSpinText;
            }
            set
            {
                if (_IsChkMakeCaptionAsSpinText == value)
                    return;
                SetProperty(ref _IsChkMakeCaptionAsSpinText, value);
            }
        }

        private bool _isChkAutoReplayPrivateBlacklist;

        public bool IsChkAutoReplyPrivateBlacklist
        {
            get
            {
                return _isChkAutoReplayPrivateBlacklist;
            }
            set
            {
                if (_isChkAutoReplayPrivateBlacklist == value)
                    return;
                SetProperty(ref _isChkAutoReplayPrivateBlacklist, value);

            }
        }

        private bool _isChkAutoReplayGroupBlacklist;

        public bool IsChkAutoReplyGroupBlacklist
        {
            get
            {
                return _isChkAutoReplayGroupBlacklist;
            }
            set
            {
                if (_isChkAutoReplayGroupBlacklist == value)
                    return;
                SetProperty(ref _isChkAutoReplayGroupBlacklist, value);

            }
        }

        private List<string> _lstMultiMessageForUserHasNotReplied = new List<string>();

        public List<string> LstMultiMessageForUserHasNotReplied
        {
            get
            {
                return _lstMultiMessageForUserHasNotReplied;
            }

            set
            {
                if (_lstMultiMessageForUserHasNotReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasNotReplied, value);
            }
        }
        private List<string> _lstMultiMessageForUserHasReplied = new List<string>();

        public List<string> LstMultiMessageForUserHasReplied
        {
            get
            {
                return _lstMultiMessageForUserHasReplied;
            }

            set
            {
                if (_lstMultiMessageForUserHasReplied != value)
                    SetProperty(ref _lstMultiMessageForUserHasReplied, value);
            }
        }
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } = new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();


    }
}
