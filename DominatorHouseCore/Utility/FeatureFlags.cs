using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Utility
{
    public class FeatureFlags : Dictionary<string, bool>
    {
        public static FeatureFlags Instance = null;
        public static bool Check(string key, Action whenEnabled = null, Action whenDisabled = null)
        {
            if (!Instance.ContainsKey(key))
            {
                Instance[key] = false;
            }

            var value = Instance[key];
            if (value)
            {
                whenEnabled?.Invoke();
            }
            else
            {
                whenDisabled?.Invoke();
            }
            return value;
        }

        public static bool Check(string key) => Instance[key];

        
        public static Visibility Check(SocialNetworks network) 
            => Instance[network.ToString()] ? Visibility.Visible : Visibility.Collapsed;
    }
}