using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountUpdateFactory
    {
        bool CheckStatus(DominatorAccountModel accountModel);

        void UpdateDetails(DominatorAccountModel accountModel);      
    }
}