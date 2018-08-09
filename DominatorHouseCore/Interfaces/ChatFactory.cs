using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public abstract class ChatFactory
    {
        public virtual void UpdateFriendList(LiveChatModel liveChatModel, CancellationToken cancellation) { }
        public virtual void UpdateCurrentChat(LiveChatModel liveChatModel, CancellationToken cancellation) { }

        public virtual async Task<bool> SendMessageToUser(LiveChatModel liveChatModel, string message, ChatMessageType messageType, CancellationToken cancellation) =>
            true;

        public void SaveChatDetails(LiveChatModel liveChatModel, ChatDetails chatDetails, CancellationToken cancellation)
        {
            var oldData = GenericFileManager.GetModuleDetails<ChatDetails>(
                FileDirPath.GetChatDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
            bool isPresent = false;
            oldData?.ForEach(chat =>
            {
                cancellation.ThrowIfCancellationRequested();
                try
                {
                    if (chat.SenderId == chatDetails.SenderId)
                    {
                        isPresent = true;
                        if (!ObjectComparer.Compare<ChatDetails>(chat, chatDetails))
                        {
                            #region Updating UI

                            var oldChatindex = liveChatModel.LstChat.IndexOf(chat);
                            liveChatModel.LstChat[oldChatindex].Sender = chatDetails.Sender;
                            liveChatModel.LstChat[oldChatindex].Messeges = chatDetails.Messeges;
                            liveChatModel.LstChat[oldChatindex].Type = chatDetails.Type;
                            liveChatModel.LstChat[oldChatindex].Time = chatDetails.Time;
                            liveChatModel.LstChat[oldChatindex].MessegesId = chatDetails.MessegesId;

                            #endregion

                            chat.Sender = chatDetails.Sender;
                            chat.Messeges = chatDetails.Messeges;
                            chat.Type = chatDetails.Type;
                            chat.Time = chatDetails.Time;
                            chat.MessegesId = chatDetails.MessegesId;
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            });
            GenericFileManager.UpdateModuleDetails<ChatDetails>(oldData,
                FileDirPath.GetChatDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
            try
            {
                if (!isPresent)
                {
                    liveChatModel.LstChat.Add(chatDetails);
                    GenericFileManager.AddModule<ChatDetails>(chatDetails,
                        FileDirPath.GetFriendDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
        public void SaveFriendDetails(LiveChatModel liveChatModel, SenderDetails friendDetail, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var oldData = GenericFileManager.GetModuleDetails<SenderDetails>(
                FileDirPath.GetFriendDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
            bool isPresent = false;
            oldData?.ForEach(friends =>
            {
                try
                {
                    cancellation.ThrowIfCancellationRequested();
                    if (friends.SenderId == friendDetail.SenderId)
                    {
                        isPresent = true;
                        if (!ObjectComparer.Compare<SenderDetails>(friends, friendDetail))
                        {
                            #region Update UI

                            var oldFriendDetail = liveChatModel.LstSender.IndexOf(friends);

                            liveChatModel.LstSender[oldFriendDetail].SenderImage = friendDetail.SenderImage;
                            liveChatModel.LstSender[oldFriendDetail].SenderId = friendDetail.SenderId;
                            liveChatModel.LstSender[oldFriendDetail].LastMesseges = friendDetail.LastMesseges;
                            liveChatModel.LstSender[oldFriendDetail].SenderName = friendDetail.SenderName;
                            liveChatModel.LstSender[oldFriendDetail].ThreadId = friendDetail.ThreadId;
                            liveChatModel.LstSender[oldFriendDetail].LastMessegedate = friendDetail.LastMessegedate;
                            liveChatModel.LstSender[oldFriendDetail].AccountId = friendDetail.AccountId;

                            #endregion

                            friends.SenderImage = friendDetail.SenderImage;
                            friends.SenderId = friendDetail.SenderId;
                            friends.LastMesseges = friendDetail.LastMesseges;
                            friends.SenderName = friendDetail.SenderName;
                            friends.ThreadId = friendDetail.ThreadId;
                            friends.LastMessegedate = friendDetail.LastMessegedate;
                            friends.AccountId = friendDetail.AccountId;

                        }
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            });
            GenericFileManager.UpdateModuleDetails<SenderDetails>(oldData,
                FileDirPath.GetFriendDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
            try
            {
                if (!isPresent)
                {
                    liveChatModel.LstSender.Add(friendDetail);
                    GenericFileManager.AddModule<SenderDetails>(friendDetail,
                        FileDirPath.GetFriendDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
    }
}