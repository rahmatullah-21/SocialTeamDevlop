using System;
using System.Collections.Generic;

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
    }
}