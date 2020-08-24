#region

using System.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IBrowserManager
    {
        bool BrowserLogin(DominatorAccountModel account, CancellationToken cancellationToken,
            LoginType loginType = LoginType.AutomationLogin, VerificationType verificationType = 0);
    }
}