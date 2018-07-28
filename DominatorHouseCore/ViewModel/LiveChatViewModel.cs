using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections.Generic;
using System.Windows.Input;
using DominatorHouseCore.Command;
using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;

namespace DominatorHouseCore.ViewModel
{
    public class LiveChatViewModel : BindableBase
    {
        public SocialNetworks SocialNetworks { get; set; }
        public LiveChatViewModel()
        {
            SendMessageCommand = new BaseCommand<object>((sender) => true, SendMessageExecute);
            UserSelectionChangedCommand = new BaseCommand<object>((sender) => true, UserSelectionChangedExecute);
            FriendSelectionChangedCommand = new BaseCommand<object>((sender) => true, FriendSelectionChangedExecute);
            AttachFileCommand = new BaseCommand<object>((sender) => true, AttachFileExecute);
            EmojiCommand = new BaseCommand<object>((sender) => true, EmojiExecute);
        }



        #region Command

        public ICommand SendMessageCommand { get; set; }
        public ICommand UserSelectionChangedCommand { get; set; }
        public ICommand FriendSelectionChangedCommand { get; set; }
        public ICommand AttachFileCommand { get; set; }
        public ICommand EmojiCommand { get; set; }

        #endregion

        #region Properties

        private LiveChatModel _liveChatModel = new LiveChatModel();

        public LiveChatModel LiveChatModel
        {
            get
            {
                return _liveChatModel;
            }
            set
            {
                if (value == _liveChatModel)
                    return;
                SetProperty(ref _liveChatModel, value);
            }
        }

        private List<DominatorAccountModel> _lstAccountModel = new List<DominatorAccountModel>();
        public List<DominatorAccountModel> LstAccountModel
        {
            get
            {
                return _lstAccountModel;
            }
            set
            {
                if (value == _lstAccountModel)
                    return;
                SetProperty(ref _lstAccountModel, value);
            }
        }

        #endregion

        #region Command Methods

        private void UserSelectionChangedExecute(object sender)
        {
            UpdateFriendList();
        }

        private void FriendSelectionChangedExecute(object sender)
        {
            if (LiveChatModel.SenderDetails != null)
            {
                try
                {
                    ThreadFactory.Instance.Start(() =>
                    {
                        Application.Current.Dispatcher.Invoke(() => LiveChatModel.LstChat.Clear());

                        SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                            .UpdateCurrentChat(LiveChatModel);

                    });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private void SendMessageExecute(object sender)
        {
            SendMesage(LiveChatModel.TextMessage, ChatMessageType.Text);
        }

        private void AttachFileExecute(object sender)
        {
            var file = FileUtilities.GetExportPath();
            SendMesage(file, ChatMessageType.Media);
        }
        private void EmojiExecute(object sender)
        {

        }


        #endregion

        #region Methods

        public void UpdateFriendList()
        {
            LiveChatModel.DominatorAccountModel =
                LstAccountModel.FirstOrDefault(x =>
                    x.UserName == LiveChatModel.SelectedAccount);
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    Application.Current.Dispatcher.Invoke(() => LiveChatModel.LstSender.Clear());
                    SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                          .UpdateFriendList(LiveChatModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void SendMesage(string message, ChatMessageType chatMessageType)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    bool isSent = SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().ChatFactory
                        .SendMessageToUser(LiveChatModel, message, chatMessageType);

                    if (isSent)
                    {
                        LiveChatModel.TextMessage = string.Empty;
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks, LiveChatModel.DominatorAccountModel.AccountBaseModel.UserName, "Chat", "message sending fail");
                    }
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
