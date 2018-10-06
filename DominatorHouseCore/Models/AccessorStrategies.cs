using DominatorHouseCore.Enums;
using System;


namespace DominatorHouseCore.Models
{
    public class AccessorStrategies
    {
        public Func<SocialNetworks, bool> _determine_available;
        public Action<string> _inform_warnings;
        public Action<DominatorAccountModel> ActionCheckAccount;
        public Action<DominatorAccountModel> AccountBrowserLogin;
        public Action<DominatorAccountModel> action_UpdateFollower;
        public Action<DominatorAccountModel> EditProfile;
        public Action<DominatorAccountModel> RemovePhoneVerification;
    }
}
