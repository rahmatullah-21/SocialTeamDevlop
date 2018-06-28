using DominatorHouseCore.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountUpdateFactory
    {
        bool CheckStatus(DominatorAccountModel accountModel);

        void UpdateDetails(DominatorAccountModel accountModel);
    }
    public interface IAccountUpdateFactoryAsync: IAccountUpdateFactory
    {
        Task<bool> CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token);

        Task UpdateDetailsAsync(DominatorAccountModel accountModel, CancellationToken token);
    }
    public interface IAccountVerificationFactory
    {
        Task<bool> VerifyAccountAsync(DominatorAccountModel accountModel, CancellationToken token);

    }
}