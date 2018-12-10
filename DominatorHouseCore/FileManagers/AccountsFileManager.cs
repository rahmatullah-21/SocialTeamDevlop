using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.FileManagers
{
    public interface IAccountsFileManager
    {
        // void SaveAll(List<DominatorAccountModel> lstAccountModel);
        void UpdateAccounts(IList<DominatorAccountModel> libraryAccounts);
        bool Edit(DominatorAccountModel account);
        List<DominatorAccountModel> GetAll();
        List<DominatorAccountModel> GetAll(SocialNetworks network);

        List<DominatorAccountModel> GetAllAccounts(List<string> neededAccountList,
            SocialNetworks socialNetwork);

        bool Add(DominatorAccountModel account);
        //  bool Add<AModel>(AModel account) where AModel : class;
        // void DeleteSelected(List<DominatorAccountModel> accs);

        void Delete(Func<DominatorAccountModel, bool> match);

        //   DominatorAccountModel GetAccount(string userName);

        DominatorAccountModel GetAccountById(string accountId);

        DominatorAccountModel GetAccount(string userName, SocialNetworks networks);

        IEnumerable<string> GetUsers();

        IEnumerable<string> GetUsers(SocialNetworks networks);
        List<DominatorAccountModel> GetAll(List<string> neededAccountList);
    }

    public class AccountsFileManager : IAccountsFileManager
    {
        private readonly IAccountsCacheService _accountsCacheService;

        public AccountsFileManager(IAccountsCacheService accountsCacheService)
        {
            _accountsCacheService = accountsCacheService;
        }


        // Saves all accounts. Have to work Only in Social library. Otherwise use UpdateAccounts() method to update AccountDetails.bin
        // NOTE: make sure lstAccountModel contains all accounts

        //internal static void SaveAll(List<DominatorAccountModel> lstAccountModel)
        //{
        //    // Warning: make sure lstAccountModel contains all accounts            
        //    AccountsCacheService.UpsertAccounts(lstAccountModel.ToArray());
        //    GlobusLogHelper.log.Debug($"{lstAccountModel.Count} Accounts successfully saved");
        //}



        // Update account entries and save to AccountDetails.bin        
        public void UpdateAccounts(IList<DominatorAccountModel> libraryAccounts)
        {
            _accountsCacheService.UpsertAccounts(libraryAccounts.ToArray());
        }

        // alias
        public bool Edit(DominatorAccountModel account)
            => SaveAccount(account);

        public List<DominatorAccountModel> GetAll() => _accountsCacheService.GetAccountDetails().ToList();

        public List<DominatorAccountModel> GetAll(SocialNetworks network)
            => _accountsCacheService.GetAccountDetails().Where(a => a.AccountBaseModel.AccountNetwork == network).ToList();

        public List<DominatorAccountModel> GetAll(List<string> neededAccountList)
            => _accountsCacheService.GetAccountDetails().Where(a => neededAccountList.Contains(a.AccountBaseModel.UserName)).ToList();

        public List<DominatorAccountModel> GetAllAccounts(List<string> neededAccountList,
            SocialNetworks socialNetwork)
        {
            var Accounts = _accountsCacheService.GetAccountDetails().Where(a => neededAccountList.Contains((a.AccountBaseModel.UserName)))
                .ToList();
            return Accounts.FindAll(x => x.AccountBaseModel.AccountNetwork == socialNetwork);
        }

        // backward compatibility for TD, PD
        public bool Add(DominatorAccountModel account)
        {
            return _accountsCacheService.UpsertAccounts(account);
        }

        // backward compatibility for TD, PD

        //public static bool Add<AModel>(AModel account) where AModel : class
        //{
        //    throw new Exception("this method should be deleted");
        //    //return BinFileHelper.Append(account);
        //}

        //public static void DeleteSelected(List<DominatorAccountModel> accs)
        //{
        //    AccountsCacheService.Delete(accs.ToArray());
        //}


        public void Delete(Func<DominatorAccountModel, bool> match)
        {
            var accs = GetAll();
            _accountsCacheService.Delete(accs.Where(match).ToArray());
        }


        //public static DominatorAccountModel GetAccount(string userName)
        //{
        //    var accounts = GetAll();
        //    var result = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == userName);
        //    return result;
        //}


        public DominatorAccountModel GetAccountById(string accountId)

        {
            var accounts = GetAll();
            var result = accounts.FirstOrDefault(x => x.AccountBaseModel.AccountId == accountId);
            return result;
        }

        public DominatorAccountModel GetAccount(string userName, SocialNetworks networks)
        {
            var accounts = GetAll();
            var result = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == userName && x.AccountBaseModel.AccountNetwork == networks);
            return result;
        }

        public IEnumerable<string> GetUsers()
        {
            var accounts = GetAll();
            var result = accounts.Select(x => x.AccountBaseModel.UserName);
            return result;
        }

        public IEnumerable<string> GetUsers(SocialNetworks networks)
        {
            var accounts = GetAll();
            var result = accounts.Where(x => x.AccountBaseModel.AccountNetwork == networks).Select(x => x.AccountBaseModel.UserName);
            return result;
        }

        // Saves one account by looking for it in list of all accounts.
        // Use Edit() in consumer code
        private bool SaveAccount(DominatorAccountModel account)
        {
            var savedStatus = _accountsCacheService.UpsertAccounts(account);

            if (savedStatus)
            {
                GlobusLogHelper.log.Debug($"Accounts successfully saved - [{account.AccountBaseModel.UserName}]");
            }
            return savedStatus;
        }

    }
}
