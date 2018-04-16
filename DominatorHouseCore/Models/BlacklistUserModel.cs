using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
   
   public class BlacklistUserModel : BindableBase
    {
        public ObservableCollection<BlacklistUserModel> LstBlackListUsers { get; set; }=new ObservableCollection<BlacklistUserModel>();


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
        private bool _isAllBlackListUserChecked;

        public bool IsAllBlackListUserChecked
        {
            get { return _isAllBlackListUserChecked; }
            set
            {
                if (value == _isAllBlackListUserChecked)
                    return;
                SetProperty(ref _isAllBlackListUserChecked, value);
            }
        }
    }
}
