using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface IBrowserManager
    {
        bool BrowserLogin(DominatorAccountModel account);
    }
}
