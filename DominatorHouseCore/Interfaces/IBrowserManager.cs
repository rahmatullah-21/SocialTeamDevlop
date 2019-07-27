using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.Threading;

namespace DominatorHouseCore.Interfaces
{
    public interface IBrowserManager
    {
        bool BrowserLogin(DominatorAccountModel account, LoginType loginType = LoginType.AutomationLogin);
    }
}
