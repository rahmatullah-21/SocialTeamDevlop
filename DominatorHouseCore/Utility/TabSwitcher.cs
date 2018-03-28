using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;

namespace DominatorHouseCore.Utility
{
    public class TabSwitcher
    {
        //dont use
        public static Action<int> SelectMainTab { get; set; }

        //dont use
        /// <summary>
        ///     Dont use this action, instead use <see cref="DominatorHouseCore.Utility.TabSwitcher.ChangeTabIndex" />
        /// </summary>
        public static Action<int, int> SelectMainTabIndex { get; set; } = (i, j) =>
            GlobusLogHelper.log.Error("ChangeTabIndex wasn't set");

        public static Action GoToCampaign { get; set; }

        public static Action<int, int?> ChangeTabIndex { get; set; } = (i, j) =>
            GlobusLogHelper.log.Error("ChangeTabIndex wasn't set");

        public static Action<int, SocialNetworks, string> ChangeTabWithNetwork { get; set; }
    }
}