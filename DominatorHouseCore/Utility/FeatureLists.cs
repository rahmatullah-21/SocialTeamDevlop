using System;
using System.Collections.Generic;

namespace DominatorHouseCore.Utility
{
    public enum RequiredActions
    {
        JobProcess = 1,
        Scraper = 2,
        TabHandler = 3,
        AccountUpdateDetails = 4,
        AccountCount = 5
    }
    public class FeatureLists
    {
        public Dictionary<string, bool> Flags { get; set; }
        public FeatureLists()
        {
            Flags = new Dictionary<string, bool>();
            foreach (RequiredActions feature in Enum.GetValues(typeof(RequiredActions)))
                Flags.Add(feature.ToString(), false);
        }
    }
}