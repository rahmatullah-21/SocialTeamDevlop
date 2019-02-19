using DominatorHouseCore.Enums;
using System;


namespace DominatorHouseCore.Models
{
    public class AccessorStrategies
    {
        public Func<SocialNetworks, bool> _determine_available;
        public Action<string> _inform_warnings;
    }
}
