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
            bool requireUpdate = false;
            foreach (var chat in oldData)
            {
                cancellation.ThrowIfCancellationRequested();
                try
                {
                    //if (chat.SenderId == chatDetails.SenderId)
                    if (chat.SenderId == chatDetails.SenderId && chat.MessegesId == chatDetails.MessegesId)
                    {
                        isPresent = true;
                        if (!ObjectComparer.Compare(chat, chatDetails))
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
                            requireUpdate = true;
                        }
                        break;
                    }

                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }


            }

            try
            {
                if (!isPresent)
                {
                    cancellation.ThrowIfCancellationRequested();
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        liveChatModel.LstChat.Add(chatDetails);
                    });
                    //liveChatModel.LstChat.Add(chatDetails);
                    GenericFileManager.AddModule(chatDetails,
                        FileDirPath.GetChatDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
                }
                else if (requireUpdate)
                {
                    GenericFileManager.UpdateModuleDetails(oldData,
                        FileDirPath.GetChatDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
                }
            }
            catch (OperationCanceledException)
            {
               
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
            bool requireUpdate = false;
            foreach (var friends in oldData)
            {
                try
                {
                    cancellation.ThrowIfCancellationRequested();
                    if (friends.SenderId == friendDetail.SenderId)
                    {
                        isPresent = true;
                        if (!ObjectComparer.Compare(friends, friendDetail))
                        {
                            #region Update UI

                            var oldFriendDetail = liveChatModel.LstSender.IndexOf(liveChatModel.LstSender.FirstOrDefault(x => x.ThreadId == friends.ThreadId));

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
                            requireUpdate = true;
                        }
                        break;
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }
            try
            {
                if (!isPresent)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        liveChatModel.LstSender.Add(friendDetail);
                    });

                    GenericFileManager.AddModule(friendDetail,
                        FileDirPath.GetFriendDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
                }
                else if (requireUpdate)
                {
                    GenericFileManager.UpdateModuleDetails(oldData,
                        FileDirPath.GetFriendDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
    }

   
    namespace DominatorHouseCore.Interfaces
    {
    }

}