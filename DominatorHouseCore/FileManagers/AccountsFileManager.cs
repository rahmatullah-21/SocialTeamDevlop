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

        public static void Save<T>(IList<T> lstAccountModel) where T : class
        {
            BinFileHelper.UpdateAllAccounts(lstAccountModel);
            GlobusLogHelper.log.Debug("Accounts successfully saved");
        }

        public static bool SaveAccount(DominatorAccountModel account)
        {
            var savedStatus=  BinFileHelper.UpdateAccount(account);

            if (savedStatus)
            {
                GlobusLogHelper.log.Debug($"Accounts successfully saved - [{account.AccountBaseModel.UserName}]");
            }

            return savedStatus;
        }


        // TODO: remove. Backward compatibility
        public static void SaveAccount<T>(T account) where T : class
        {
            BinFileHelper.UpdateAccount<T>(account);
            GlobusLogHelper.log.Debug($"Accounts successfully saved - [{(account as dynamic).UserName}]");
        }


        private static readonly object _addAccountFileLocker = new object();

        public static bool AddNewAccount(DominatorAccountModel account)
        {
            try
            {
                lock (_addAccountFileLocker)
                {
                    var accountDetailsList = new List<DominatorAccountModel>();
                    accountDetailsList.Add(account);
                    bool result = ProtoBuffBase.SerializeObjects(accountDetailsList,
                        ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}");

                    GlobusLogHelper.log.Trace($"Add Accounts - [{result}]");
                    return result;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Add account details error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
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

        public static List<DominatorAccountModel> Get()
        {
            var result = BinFileHelper.GetAccountDetails();

            return result;
        }


        // Back compatibility for old account models
        public static List<AccountModel> GetFor<AccountModel>() where AccountModel : class
            => BinFileHelper.GetAccountDetailsFor<AccountModel>();


        public static DominatorAccountModel GetAccount(string userName)
        {
            var accounts = Get();
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
            var result = BinFileHelper.GetUsers(DominatorHouseInitializer.ActiveSocialNetwork);

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
