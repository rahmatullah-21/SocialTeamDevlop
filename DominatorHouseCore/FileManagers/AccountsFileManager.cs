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

        internal static void SaveAll<T>(List<T> lstAccountModel) where T : class
        {
#if DEBUG
            // make sure lstAccountModel contains all accounts
            //var all = GetAll();
            //Debug.Assert(all.Count == lstAccountModel.Count);      
#endif

            BinFileHelper.UpdateAllAccounts(lstAccountModel);
            GlobusLogHelper.log.Debug("Accounts successfully saved");
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

        // Saves one account by looking for it in list of all accounts
        public static bool SaveAccount(DominatorAccountModel account)
        {
            var savedStatus = BinFileHelper.UpdateAccount(account);

            if (savedStatus)
            {
                GlobusLogHelper.log.Debug($"Accounts successfully saved - [{account.AccountBaseModel.UserName}]");
            }

            return savedStatus;
        }


        // For other than DominatorAccountModel
        public static bool SaveAccount<T>(T account) where T : class
        {
            Debug.Assert(typeof(T).Name.Contains("AccountModel"));

            var savedStatus = BinFileHelper.UpdateAccount<T>(account);
            GlobusLogHelper.log.Debug($"Accounts successfully saved");

            return savedStatus;
        }

        public static void FillList<T>(ObservableCollection<T> lstAccountModel) where T : class
        {
            lstAccountModel.Clear();
            ApplyFunc(a =>
            {
                lstAccountModel.Add((T)(object)a);
                return false;
            });
        }

        public static List<DominatorAccountModel> GetAll()
        {
            return BinFileHelper.GetAccountDetails();           
        }

        public static List<DominatorAccountModel> GetAll(SocialNetworks network)
        {
            return BinFileHelper.GetAccountDetails().Where(a => a.AccountBaseModel.AccountNetwork == network).ToList();
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

        public static void Delete<AModel>(Predicate<AModel> match) where AModel : class
        {
            var accs = GetFor<AModel>();
            var toDelete = accs.FindAll(match);
            accs.RemoveAll(match);
            BinFileHelper.UpdateAllAccounts(accs);            
        }

        // alias
        public static void Edit<TModel>(TModel account) where TModel : class => SaveAccount(account);


        // Back compatibility for old account models
        public static List<AccountModel> GetFor<AccountModel>() where AccountModel : class
            => BinFileHelper.GetAccountDetailsFor<AccountModel>();


        public static DominatorAccountModel GetAccount(string userName)
        {
            var accounts = GetAll();
            var result = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == userName);

            return result;
        }

        // TODO: remove. backward compatibility for old account models
        public static T GetAccount<T>(string userName) where T : class
        {
            var accounts = GetFor<T>();

            var result = accounts.FirstOrDefault(x => (x as dynamic).UserName == userName);
            return result;
        }

        public static ObservableCollectionBase<string> GetUsers()
        {
            var result = BinFileHelper.GetUsers();

            return result;
        }

        // TODO: remove. backward compatibility for old account models
        public static ObservableCollectionBase<string> GetUsersFor<T>() where T : class
        {
            var result = BinFileHelper.GetUsers<T>();

            return result;
        }


        

    }
}
