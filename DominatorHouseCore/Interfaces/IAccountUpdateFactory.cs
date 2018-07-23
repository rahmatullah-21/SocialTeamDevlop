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
    public interface IAccountUpdateFactoryAsync: IAccountUpdateFactory
    {
        Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token);

        Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token);
    }
    public interface IAccountVerificationFactory
    {
        Task<bool> VerifyAccountAsync(DominatorAccountModel accountModel, VerificationType verificationType, CancellationToken token);
        Task<bool> SendVerificationCode(DominatorAccountModel accountModel, VerificationType verificationType, CancellationToken token);
       
    }
}