using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class WhitelistUserModel : BindableBase
    {
        public ObservableCollection<WhitelistUserModel> LstWhiteListUsers { get; set; } = new ObservableCollection<WhitelistUserModel>();


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
        private bool _isAllWhiteListUserChecked;

        public bool IsAllWhiteListUserChecked
        {
            get { return _isAllWhiteListUserChecked; }
            set
            {
                if (value == _isAllWhiteListUserChecked)
                    return;
                SetProperty(ref _isAllWhiteListUserChecked, value);
            }
        }
    }
}
