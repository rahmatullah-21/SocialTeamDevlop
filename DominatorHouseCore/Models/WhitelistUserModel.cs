using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class WhitelistUserModel : BindableBase
    {
      

        private string _whitelistUser;

        public string WhitelistUser
        {
            get
            {
                return _whitelistUser;
            }
            set
            {
                if (value == _whitelistUser)
                    return;
                SetProperty(ref _whitelistUser, value);
            }
        }
        private bool _isWhiteListUserChecked;

        public bool IsWhiteListUserChecked
        {
            get { return _isWhiteListUserChecked; }
            set
            {
                if (value == _isWhiteListUserChecked)
                    return;
                SetProperty(ref _isWhiteListUserChecked, value);
            }
        }
       
    }
}
