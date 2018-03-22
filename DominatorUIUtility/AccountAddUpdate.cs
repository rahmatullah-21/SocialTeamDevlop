using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorUIUtility
{
   public class AccountAddUpdate
    {
        public static Action<DominatorAccountModel> UpdateAccount { get; set; }
        public static Action<DominatorAccountModel> UpdateQDAccount { get; set; }
        public static Action<DominatorAccountModel> UpdateGDAccount { get; set; }
    }
}
