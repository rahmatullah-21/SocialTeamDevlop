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
       public static Action<int, int> ChangeTabIndex { get; set; }
       public static Action<int,SocialNetworks,string> ChangeTabWithNetwork { get; set; }
        
    }
}
