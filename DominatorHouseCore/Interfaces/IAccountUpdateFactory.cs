using DominatorHouseCore.Models;
using DominatorHouseCore.ViewModel;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.TdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;

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
        public virtual void UpdateFriendList(LiveChatModel liveChatModel,CancellationToken cancellation) { }
        public virtual void UpdateCurrentChat(LiveChatModel liveChatModel, CancellationToken cancellation) { }

        public virtual async Task<bool> SendMessageToUser(LiveChatModel liveChatModel, string message, ChatMessageType messageType, CancellationToken cancellation) =>
            true;
    }

}