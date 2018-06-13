using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    class TwitterModel : BindableBase
    {
        private bool _isEnableFollowDifferentUsersAcrossTwitterAccountsChecked;
        [ProtoMember(1)]
        public bool IsEnableFollowDifferentUsersAcrossTwitterAccountsChecked
        {
            get
            {
                return _isEnableFollowDifferentUsersAcrossTwitterAccountsChecked;
            }
            set
            {
                if (value == _isEnableFollowDifferentUsersAcrossTwitterAccountsChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUsersAcrossTwitterAccountsChecked, value);
            }
        }
        private bool _isEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked;
        [ProtoMember(2)]
        public bool IsEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked
        {
            get
            {
                return _isEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked;
            }
            set
            {
                if (value == _isEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUsersAcrossTwitterAccountsWithSameTagnameChecked, value);
            }
        }
    }
}
