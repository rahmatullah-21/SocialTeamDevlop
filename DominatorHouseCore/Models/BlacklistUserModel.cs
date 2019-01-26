using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class BlacklistUserModel : BindableBase
    {

        private string _blacklistUser;

        public string BlacklistUser
        {
            get
            {
                return _blacklistUser;
            }
            set
            {
                if (value == _blacklistUser)
                    return;
                SetProperty(ref _blacklistUser, value);
            }
        }
        private bool _isBlackListUserChecked;

        public bool IsBlackListUserChecked
        {
            get { return _isBlackListUserChecked; }
            set
            {
                if (value == _isBlackListUserChecked)
                    return;
                SetProperty(ref _isBlackListUserChecked, value);
            }
        }

    }
}
