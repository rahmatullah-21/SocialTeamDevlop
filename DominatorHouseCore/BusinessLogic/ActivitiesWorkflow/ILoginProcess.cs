using DominatorHouseCore.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.BusinessLogic.ActivitiesWorkflow
{
    /// <summary>
    /// Interface for log-in for any social network.
    /// </summary>
    public interface ILoginProcess
    {
        bool CheckLogin(DominatorAccountModel dominatorAccountModel);

        void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired);

        void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel);
    }

    public interface ILoginProcessAsync : ILoginProcess
    {
        Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken);

        Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel, bool isMobileRequired, CancellationToken cancellationToken);

        Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken);
    }
}