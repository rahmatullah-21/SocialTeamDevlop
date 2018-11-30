using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.FileManagers
{
    public interface IAccountsCacheService
    {
        IReadOnlyCollection<DominatorAccountModel> GetAccountDetails();
        bool UpsertAccounts(params DominatorAccountModel[] accounts);
        bool Delete(params DominatorAccountModel[] accounts);
    }

    public class AccountsCacheService : IAccountsCacheService
    {
        private readonly object _syncContext = new object();
        private readonly Lazy<Dictionary<string, DominatorAccountModel>> _cache;
        private readonly IBinFileHelper _binFileHelper;

        public AccountsCacheService(IBinFileHelper binFileHelper)
        {
            _binFileHelper = binFileHelper;
            _cache = new Lazy<Dictionary<string, DominatorAccountModel>>(() =>
            {
                return _binFileHelper.GetAccountDetails().ToDictionary(a => a.AccountId, a => a);
            });
        }

        public IReadOnlyCollection<DominatorAccountModel> GetAccountDetails()
        {
            lock (_syncContext)
            {
                return _cache.Value.Values;
            }
        }

        public bool UpsertAccounts(params DominatorAccountModel[] accounts)
        {
            lock (_syncContext)
            {
                var cacheCopy = _cache.Value.ToDictionary(a => a.Key, a => a.Value);
                UpsertData(cacheCopy, accounts);
                var result = _binFileHelper.UpdateAllAccounts(cacheCopy.Values.ToList());
                if (result)
                {
                    _cache.Value.Clear();
                    foreach (var model in cacheCopy)
                    {
                        _cache.Value.Add(model.Key, model.Value);
                    }
                }

                return result;
            }
        }

        public bool Delete(params DominatorAccountModel[] accounts)
        {
            lock (_syncContext)
            {
                var cacheCopy = _cache.Value.ToDictionary(a => a.Key, a => a.Value);
                foreach (var model in accounts)
                {
                    if (cacheCopy.ContainsKey(model.AccountId))
                        cacheCopy.Remove(model.AccountId);
                }
                var result = _binFileHelper.UpdateAllAccounts(cacheCopy.Values.ToList());
                if (result)
                {
                    _cache.Value.Clear();
                    foreach (var model in cacheCopy)
                    {
                        _cache.Value.Add(model.Key, model.Value);
                    }
                }
                return result;
            }
        }

        private void UpsertData(IDictionary<string, DominatorAccountModel> target, params DominatorAccountModel[] source)
        {
            lock (_syncContext)
            {
                foreach (var account in source)
                {
                    if (target.ContainsKey(account.AccountId))
                    {
                        target[account.AccountId] = account;
                    }
                    else
                    {
                        target.Add(account.AccountId, account);
                    }
                }
            }
        }
    }
}
