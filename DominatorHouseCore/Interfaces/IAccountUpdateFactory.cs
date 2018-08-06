using System;
using System.Linq;
using DominatorHouseCore.Models;
using DominatorHouseCore.ViewModel;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountUpdateFactory
    {
        bool CheckStatus(DominatorAccountModel accountModel);

        void UpdateDetails(DominatorAccountModel accountModel);

        DailyStatisticsViewModel GetDailyGrowth(string accuntId, string username, GrowthPeriod period);
    }
    public interface IAccountUpdateFactoryAsync : IAccountUpdateFactory
    {
        Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token);

        Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token);
    }
    public interface IAccountVerificationFactory
    {
        Task<bool> VerifyAccountAsync(DominatorAccountModel accountModel, VerificationType verificationType, CancellationToken token);
        Task<bool> SendVerificationCode(DominatorAccountModel accountModel, VerificationType verificationType, CancellationToken token);

    }

    public abstract class ProfileFactory
    {
        public virtual void EditProfile(DominatorAccountModel accountModel) { }
        public virtual void RemovePhoneVerification(DominatorAccountModel accountModel) { }
    }
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
            oldData.ForEach(chat =>
            {
                cancellation.ThrowIfCancellationRequested();
                try
                {
                   if (chat.SenderId == chatDetails.SenderId)
                    {
                        if (!ObjectComparer.Compare<ChatDetails>(chat, chatDetails))
                        {
                            var oldChatindex = liveChatModel.LstChat.IndexOf(chat);
                            liveChatModel.LstChat[oldChatindex].Sender = chatDetails.Sender;
                            liveChatModel.LstChat[oldChatindex].Messeges = chatDetails.Messeges;
                            liveChatModel.LstChat[oldChatindex].Type = chatDetails.Type;
                            liveChatModel.LstChat[oldChatindex].Time = chatDetails.Time;
                            liveChatModel.LstChat[oldChatindex].MessegesId = chatDetails.MessegesId;
                        }
                    }
                    else
                    {
                        liveChatModel.LstChat.Add(chatDetails);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            });
            GenericFileManager.AddRangeModule<ChatDetails>(liveChatModel.LstChat.ToList(),
                FileDirPath.GetChatDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));

        }
        public void SaveFriendDetails(LiveChatModel liveChatModel, SenderDetails friendDetail, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var oldData = GenericFileManager.GetModuleDetails<SenderDetails>(
                FileDirPath.GetFriendDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
            oldData.ForEach(friends =>
            {
                cancellation.ThrowIfCancellationRequested();
                try
                {
                   
                    if (friends.SenderId == friendDetail.SenderId)
                    {
                        if (!ObjectComparer.Compare<SenderDetails>(friends, friendDetail))
                        {
                            var oldFriendDetail = liveChatModel.LstSender.IndexOf(friends);

                            liveChatModel.LstSender[oldFriendDetail].SenderImage = friendDetail.SenderImage;
                            liveChatModel.LstSender[oldFriendDetail].SenderId = friendDetail.SenderId;
                            liveChatModel.LstSender[oldFriendDetail].LastMesseges = friendDetail.LastMesseges;
                            liveChatModel.LstSender[oldFriendDetail].SenderName = friendDetail.SenderName;
                            liveChatModel.LstSender[oldFriendDetail].ThreadId = friendDetail.ThreadId;
                            liveChatModel.LstSender[oldFriendDetail].LastMessegedate = friendDetail.LastMessegedate;
                            liveChatModel.LstSender[oldFriendDetail].AccountId = friendDetail.AccountId;
                        }
                    }
                    else
                    {
                        liveChatModel.LstSender.Add(friendDetail);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            });
            GenericFileManager.AddRangeModule<SenderDetails>(liveChatModel.LstSender.ToList(),
                FileDirPath.GetFriendDetailFile(liveChatModel.DominatorAccountModel.AccountBaseModel.AccountNetwork));
        }
    }

}