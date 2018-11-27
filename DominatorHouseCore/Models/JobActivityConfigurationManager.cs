using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;
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
        private readonly Dictionary<string, Dictionary<ActivityType, ModuleConfiguration>> _configurations;
        private readonly object _syncObject = new object();

        public JobActivityConfigurationManager()
        {
            _configurations = new Dictionary<string, Dictionary<ActivityType, ModuleConfiguration>>();
        }


        public ModuleConfiguration this[string accountId, ActivityType activityType]
        {
            get
            {
                lock (_syncObject)
                {
                    if (_configurations.ContainsKey(accountId))
                    {
                        if (_configurations[accountId].ContainsKey(activityType))
                        {
                            return _configurations[accountId][activityType];
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
                    if (_configurations.ContainsKey(accountId))
                    {
                        return _configurations[accountId].Values;
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
                if (_configurations.ContainsKey(accountId))
                {
                    if (_configurations[accountId].ContainsKey(activityType))
                    {
                        _configurations[accountId][activityType] = value;
                    }
                    else
                    {
                        _configurations[accountId].Add(activityType, value);
                    }
                }
                else
                {
                    _configurations.Add(accountId, new Dictionary<ActivityType, ModuleConfiguration>
                    {
                        {activityType, value}
                    });
                }
            }
        }

        public void Delete(string accountId, ActivityType activityType)
        {
            lock (_syncObject)
            {
                if (_configurations.ContainsKey(accountId))
                {
                    if (_configurations[accountId].ContainsKey(activityType))
                    {
                        _configurations[accountId].Remove(activityType);
                    }
                }
            }
        }

        public IReadOnlyCollection<ModuleConfiguration> AllEnabled()
        {
            lock (_syncObject)
            {
                return _configurations.Values.SelectMany(a => a.Values)
                    .Where(a => a.IsEnabled && a.LstRunningTimes != null).ToList();
            }
        }
    }
}
