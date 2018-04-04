using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    class TumblrModel : BindableBase
    {
        private bool _isEnableFollowDifferentUsersAcrossTumblrAccountsChecked;
        [ProtoMember(1)]
        public bool IsEnableFollowDifferentUsersAcrossTumblrAccountsChecked
        {
            get
            {
                return _isEnableFollowDifferentUsersAcrossTumblrAccountsChecked;
            }
            set
            {
                if (value == _isEnableFollowDifferentUsersAcrossTumblrAccountsChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUsersAcrossTumblrAccountsChecked, value);
            }
        }
    }
}
