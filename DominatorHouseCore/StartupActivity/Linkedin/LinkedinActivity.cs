using DominatorHouseCore.Interfaces.StartUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    public class LinkedinActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return null;
                default:
                    return null;
            }
        }

    }
}
