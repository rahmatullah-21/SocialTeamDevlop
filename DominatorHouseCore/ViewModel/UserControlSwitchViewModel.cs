using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel
{
    public abstract class UserControlSwitchViewModel : BindableBase
    {
        public abstract string Name { get;  }
    }
}
