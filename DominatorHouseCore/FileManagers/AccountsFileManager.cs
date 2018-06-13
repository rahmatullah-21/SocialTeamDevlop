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
using DominatorHouseCore.Enums;
using System.Diagnostics;

namespace DominatorHouseCore.FileManagers
{
    public static class AccountsFileManager
    {
        private static List<DominatorAccountModel> _allAccountsCache = new List<DominatorAccountModel>();

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


        // Saves all accounts. Have to work Only in Social library. Otherwise use UpdateAccounts() method to update AccountDetails.bin
        // NOTE: make sure lstAccountModel contains all accounts
        internal static void SaveAll(List<DominatorAccountModel> lstAccountModel)
        {
            // Warning: make sure lstAccountModel contains all accounts            
            BinFileHelper.UpdateAllAccounts(lstAccountModel);
            GlobusLogHelper.log.Debug($"{lstAccountModel.Count} Accounts successfully saved");
        }


        // Update account entries and save to AccountDetails.bin        
        public static void UpdateAccounts(IList<DominatorAccountModel> libraryAccounts)
        {
            var all = BinFileHelper.GetAccountDetails();

            // Update all entries that exists in libraryAccount, and add that does not exists
            for (int i = 0; i < libraryAccounts.Count; i++)
            {
                var acc = libraryAccounts[i];
                var ix = all.FindIndex(a => acc.AccountBaseModel.AccountId == a.AccountBaseModel.AccountId);
                if (ix == -1)
                    all.Add(acc);
                else
                    all[ix] = acc;
            }

            BinFileHelper.UpdateAllAccounts(all);
        }

        // Saves one account by looking for it in list of all accounts.
        // Use Edit() in consumer code
        private static bool SaveAccount(DominatorAccountModel account)
        {
            var savedStatus = BinFileHelper.UpdateAccount(account);

            if (savedStatus)
            {
                GlobusLogHelper.log.Debug($"Accounts successfully saved - [{account.AccountBaseModel.UserName}]");
            }
            return savedStatus;
        }

        // alias
        internal static bool Edit(DominatorAccountModel account)
            => SaveAccount(account);

        public static void FillList<T>(ObservableCollection<T> lstAccountModel) where T : class
        {
            lstAccountModel.Clear();
            ApplyFunc(a =>
            {
                lstAccountModel.Add((T)(object)a);
                return false;
            });
        }

        public static List<DominatorAccountModel> GetAll()  => BinFileHelper.GetAccountDetails();

        // for internal user to prevent overwriting all accounts after GetAll
        internal static List<DominatorAccountModel> GetAll(SocialNetworks network)
            => BinFileHelper.GetAccountDetails().Where(a => a.AccountBaseModel.AccountNetwork == network).ToList();

        internal static List<DominatorAccountModel> GetAll(List<string> neededAccountList) 
            => BinFileHelper.GetAccountDetails().Where(a => neededAccountList.Contains(a.AccountBaseModel.UserName)).ToList();

        internal static List<DominatorAccountModel> GetAllAccounts(List<string> neededAccountList,
            SocialNetworks socialNetwork)
        {
            var Accounts = BinFileHelper.GetAccountDetails().Where(a => neededAccountList.Contains((a.AccountBaseModel.UserName)))
                .ToList();
            return Accounts.FindAll(x => x.AccountBaseModel.AccountNetwork == socialNetwork);
        }

        // backward compatibility for TD, PD
        public static bool Add(DominatorAccountModel account)
        {
            var lst = GetAll() ?? new List<DominatorAccountModel>();
            lst.Add(account);
            BinFileHelper.UpdateAllAccounts(lst);
            return true;
        }

        // backward compatibility for TD, PD
        public static bool Add<AModel>(AModel account) where AModel : class
        {
            return BinFileHelper.Append(account);
        }

        public static void DeleteSelected(List<DominatorAccountModel> accs)
        {
            var all = GetAll().Where(a => accs.FirstOrDefault(p => p.AccountId == a.AccountId) == null).ToList();
            SaveAll(all);
        }

        public static void Delete(Predicate<DominatorAccountModel> match)
        {
            var accs = GetAll();
            accs.RemoveAll(match);
            BinFileHelper.UpdateAllAccounts(accs);
        }

        public static DominatorAccountModel GetAccount(string userName)
        {
            var accounts = GetAll();
            var result = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == userName);
            return result;
        }

        public static DominatorAccountModel GetAccountById(string accountId)

        {
            var accounts = GetAll();
            var result = accounts.FirstOrDefault(x => x.AccountBaseModel.AccountId == accountId);
            return result;
        }

        public static DominatorAccountModel GetAccount(string userName, SocialNetworks networks)
        {
            var accounts = GetAll();
            var result = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == userName && x.AccountBaseModel.AccountNetwork == networks);
            return result;
        }

        public static IEnumerable<string> GetUsers()
        {
            var accounts = GetAll();
            var result = accounts.Select(x => x.AccountBaseModel.UserName);
            return result;
        }

        public static IEnumerable<string> GetUsers(SocialNetworks networks)
        {
            var accounts = GetAll();
            var result = accounts.Where(x => x.AccountBaseModel.AccountNetwork == networks).Select(x => x.AccountBaseModel.UserName);
            return result;
        }
    }
}
