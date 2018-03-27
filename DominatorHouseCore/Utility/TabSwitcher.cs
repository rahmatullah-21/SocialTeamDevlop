using DominatorHouseCore.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Utility
{
    public class TabSwitcher
    {
    
       public static Action<int,SocialNetworks,string> ChangeTabWithNetwork { get; set; }

        //dont use
        public static Action<int> SelectMainTab { get; set; }

        //dont use
        public static Action<int, int> SelectMainTabIndex { get; set; } = (i, j) =>
            GlobusLogHelper.log.Error("ChangeTabIndex wasn't set");



        public static Action GoToCampaign { get; set; }

        public static Action<int, int?> ChangeTabIndex { get; set; } = (i, j) =>
            GlobusLogHelper.log.Error("ChangeTabIndex wasn't set");
    }



}
