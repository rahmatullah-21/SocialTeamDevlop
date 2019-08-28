using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

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

        void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel, VerificationType verificationType = 0);
    }
}