using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;

namespace DominatorHouseCore.FileManagers
{
    public static class AccountsFileManager
    {
        private static readonly IAccountsCacheService AccountsCacheService;

        static AccountsFileManager()
        {
            AccountsCacheService = ServiceLocator.Current.GetInstance<IAccountsCacheService>();
        }


        // Saves all accounts. Have to work Only in Social library. Otherwise use UpdateAccounts() method to update AccountDetails.bin
        // NOTE: make sure lstAccountModel contains all accounts
        internal static void SaveAll(List<DominatorAccountModel> lstAccountModel)
        {
            // Warning: make sure lstAccountModel contains all accounts            
            AccountsCacheService.UpsertAccounts(lstAccountModel.ToArray());
            GlobusLogHelper.log.Debug($"{lstAccountModel.Count} Accounts successfully saved");
        }


        // Update account entries and save to AccountDetails.bin        
        public static void UpdateAccounts(IList<DominatorAccountModel> libraryAccounts)
        {
            AccountsCacheService.UpsertAccounts(libraryAccounts.ToArray());
        }

        // Saves one account by looking for it in list of all accounts.
        // Use Edit() in consumer code
        private static bool SaveAccount(DominatorAccountModel account)
        {
            var savedStatus = AccountsCacheService.UpsertAccounts(account);

            if (savedStatus)
            {
                GlobusLogHelper.log.Debug($"Accounts successfully saved - [{account.AccountBaseModel.UserName}]");
            }
            return savedStatus;
        }

        // alias
        internal static bool Edit(DominatorAccountModel account)
            => SaveAccount(account);

        public static List<DominatorAccountModel> GetAll() => AccountsCacheService.GetAccountDetails().ToList();

        // for internal user to prevent overwriting all accounts after GetAll
        internal static List<DominatorAccountModel> GetAll(SocialNetworks network)
            => AccountsCacheService.GetAccountDetails().Where(a => a.AccountBaseModel.AccountNetwork == network).ToList();

        internal static List<DominatorAccountModel> GetAll(List<string> neededAccountList)
            => AccountsCacheService.GetAccountDetails().Where(a => neededAccountList.Contains(a.AccountBaseModel.UserName)).ToList();

        internal static List<DominatorAccountModel> GetAllAccounts(List<string> neededAccountList,
            SocialNetworks socialNetwork)
        {
            var Accounts = AccountsCacheService.GetAccountDetails().Where(a => neededAccountList.Contains((a.AccountBaseModel.UserName)))
                .ToList();
            return Accounts.FindAll(x => x.AccountBaseModel.AccountNetwork == socialNetwork);
        }

        // backward compatibility for TD, PD
        public static bool Add(DominatorAccountModel account)
        {
            return AccountsCacheService.UpsertAccounts(account);
        }

        // backward compatibility for TD, PD
        public static bool Add<AModel>(AModel account) where AModel : class
        {
            throw new Exception("this method should be deleted");
            return BinFileHelper.Append(account);
        }

        public static void DeleteSelected(List<DominatorAccountModel> accs)
        {
            AccountsCacheService.Delete(accs.ToArray());
        }

        public static void Delete(Func<DominatorAccountModel, bool> match)
        {
            var accs = GetAll();
            AccountsCacheService.Delete(accs.Where(match).ToArray());
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
