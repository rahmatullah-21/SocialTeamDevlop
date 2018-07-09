using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore.Diagnostics;
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

        public static void UpdateFeatures()
        {
            Instance = new FeatureFlags { { "SocinatorInitializer", true } };

            SocinatorInitialize.AvailableNetworks.ForEach(networks =>
            {
                Instance.Add(networks.ToString(),true);
            });
        }

        public static bool Check(string key) => Instance.ContainsKey(key);

        public static Visibility Check(SocialNetworks network)
            => Instance.ContainsKey(network.ToString()) ? Visibility.Visible : Visibility.Collapsed;
    }
}