using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.FileManagers
{
    public class AccountsFileManager
    {
        // Updates Accounts with applying action to it and writes changes back to file
        public static void ApplyAction(Action<DominatorAccountModel> actionToApply)
        {
            var accounts = BinFileHelper.GetAccountDetails().ToList();

            foreach (var a in accounts)
                actionToApply(a);

            BinFileHelper.UpdateAllAccounts(accounts);
        }

        // Same as above, but Func must return true if file needs to be overwritten        
        public static void ApplyFunc(Func<DominatorAccountModel, bool> funcToApply)
        {
            bool updated = false;
            var accounts = BinFileHelper.GetAccountDetails().ToList();

            foreach (var a in accounts)
                updated |= funcToApply(a);

            if (updated)
                BinFileHelper.UpdateAllAccounts(accounts);            
        }
    }
}
