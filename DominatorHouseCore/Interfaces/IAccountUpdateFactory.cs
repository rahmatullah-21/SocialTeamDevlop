using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountUpdateFactory
    {
        bool CheckStatus(DominatorAccountModel accountModel);

        void UpdateDetails(DominatorAccountModel accountModel);

        //long HeaderColumn1Value { get; set; }

        //bool HeaderColumn1Visiblity
        //GridHeaderColumn2
        //    GridHeaderColumn3
        //GridHeaderColumn3
        //    GridHeaderColumn4
        //GridHeaderColumn4

    }
}