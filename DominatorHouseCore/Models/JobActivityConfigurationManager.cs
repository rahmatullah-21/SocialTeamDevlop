using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.Models
{
    public interface IJobActivityConfigurationManager
    {
        ModuleConfiguration this[string accountId, ActivityType activityType] { get; }
        IReadOnlyCollection<ModuleConfiguration> this[string accountId] { get; }

        void AddOrUpdate(string accountId, ActivityType activityType, ModuleConfiguration value);
        void Delete(string accountId, ActivityType activityType);

        IReadOnlyCollection<ModuleConfiguration> AllEnabled();
    }

    [ProtoContract]
    public class JobActivityConfigurationManager : BindableBase, IJobActivityConfigurationManager
    {
        private readonly Lazy<Dictionary<string, Dictionary<ActivityType, ModuleConfiguration>>> _configurations;
        private readonly IAccountsCacheService _accountsCacheService;
        private readonly object _syncObject = new object();

        public JobActivityConfigurationManager(IAccountsCacheService accountsCacheService)
        {
            _accountsCacheService = accountsCacheService;
            _configurations = new Lazy<Dictionary<string, Dictionary<ActivityType, ModuleConfiguration>>>(GetModuleConfigurationAsDictionary);

            _accountsCacheService.CacheUpdated += OnAccountsCacheUpdated;
        }

        public ModuleConfiguration this[string accountId, ActivityType activityType]
        {
            get
            {
                lock (_syncObject)
                {
                    if (_configurations.Value.ContainsKey(accountId))
                    {
                        if (_configurations.Value[accountId].ContainsKey(activityType))
                        {
                            return _configurations.Value[accountId][activityType];
                        }
                    }

                    return null;
                }
            }
        }

        public IReadOnlyCollection<ModuleConfiguration> this[string accountId]
        {
            get
            {
                lock (_syncObject)
                {
                    if (_configurations.Value.ContainsKey(accountId))
                    {
                        return _configurations.Value[accountId].Values;
                    }
                    else
                    {
                        return new ModuleConfiguration[0];

                    }
                }
            }
        }

        public void AddOrUpdate(string accountId, ActivityType activityType, ModuleConfiguration value)
        {
            lock (_syncObject)
            {
                if (_configurations.Value.ContainsKey(accountId))
                {
                    if (_configurations.Value[accountId].ContainsKey(activityType))
                    {
                        _configurations.Value[accountId][activityType] = value;
                    }
                    else
                    {
                        _configurations.Value[accountId].Add(activityType, value);
                    }
                }
                else
                {
                    _configurations.Value.Add(accountId, new Dictionary<ActivityType, ModuleConfiguration>
                    {
                        {activityType, value}
                    });
                }

                _accountsCacheService[accountId].ActivityManager.AddOrUpdateModuleConfig(value);
            }
        }

        public void Delete(string accountId, ActivityType activityType)
        {
            lock (_syncObject)
            {
                if (_configurations.Value.ContainsKey(accountId))
                {
                    if (_configurations.Value[accountId].ContainsKey(activityType))
                    {
                        _configurations.Value[accountId].Remove(activityType);
                        _accountsCacheService[accountId].ActivityManager.DeleteModuleConfig(activityType);
                    }
                }
            }
        }

        public IReadOnlyCollection<ModuleConfiguration> AllEnabled()
        {
            lock (_syncObject)
            {
                return _configurations.Value.Values.SelectMany(a => a.Values)
                    .Where(a => a.IsEnabled && a.LstRunningTimes != null).ToList();
            }
        }

        private void OnAccountsCacheUpdated(object sender, IEnumerable<DominatorAccountModel> e)
        {
            lock (_syncObject)
            {
                _configurations.Value.Clear();
                foreach (var value in GetModuleConfigurationAsDictionary())
                {
                    _configurations.Value.Add(value.Key, value.Value);
                }
            }
        }

        private Dictionary<string, Dictionary<ActivityType, ModuleConfiguration>> GetModuleConfigurationAsDictionary()
        {
            return _accountsCacheService.GetAccountDetails().ToDictionary(a => a.AccountBaseModel.AccountId,
                a => a.ActivityManager.LstModuleConfiguration.GroupBy(b => b.ActivityType).ToDictionary(b => b.Key, b => b.Last()));
        }
    }
}
