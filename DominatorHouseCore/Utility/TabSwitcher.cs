using DominatorHouseCore.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{
    public class TabSwitcher
    {
        public static Action<int, int?> ChangeTabIndex { get; set; } = (i, j) =>
            GlobusLogHelper.log.Error("ChangeTabIndex wasn't set");
        
    }
}
