using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Diagnostics;

namespace DominatorHouseCore.FileManagers
{
    public class AccountsFileManager
    {
        // Updates Accounts with applying action to it and writes changes back to file
        public static void ApplyAction(Action<DominatorAccountModel> actionToApply)
        {
            var accounts = BinFileHelper.GetAccountDetails();

            foreach (var a in accounts)
                actionToApply(a);

            BinFileHelper.UpdateAllAccounts(accounts);
        }

        // Same as above, but Func must return true if file needs to be overwritten        
        public static void ApplyFunc(Func<DominatorAccountModel, bool> funcToApply)
        {
            bool updated = false;
            var accounts = BinFileHelper.GetAccountDetails();

            foreach (var a in accounts)
                updated |= funcToApply(a);

            if (updated)
                BinFileHelper.UpdateAllAccounts(accounts);
        }


        public static void Save(IList<DominatorAccountModel> lstAccountModel)
        {
            BinFileHelper.UpdateAllAccounts(lstAccountModel);
            GlobusLogHelper.log.Debug("Accounts successfully saved");
        }

        public static void SaveAccount(DominatorAccountModel account)
        {
            BinFileHelper.UpdateAccount(account);
            GlobusLogHelper.log.Debug($"Accounts successfully saved - [{account.AccountBaseModel.UserName}]");
        }

        public static void FillList(ObservableCollection<DominatorAccountModel> lstAccountModel)
        {
            lstAccountModel.Clear();
            ApplyFunc(a =>
            {
                lstAccountModel.Add(a);
                return false;
            });
        }

        public static List<DominatorAccountModel> Get()
        {
            var result = BinFileHelper.GetAccountDetails();

            return result;
        }

        public static DominatorAccountModel GetAccount(string userName)
        {
            var accounts = Get();
            var result = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == userName);

            return result;
        }

        public static ObservableCollectionBase<string> GetUsers()
        {
            var result = BinFileHelper.GetUsers(DominatorHouseInitializer.ActiveSocialNetwork);

            return result;
        }
    }
}
