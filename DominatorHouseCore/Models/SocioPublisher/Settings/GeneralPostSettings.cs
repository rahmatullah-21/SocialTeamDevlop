using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class GeneralPostSettings : BindableBase, IGeneralPostSettings
    {
        private bool _isExpireDate;
        [ProtoMember(1)]
        public bool IsExpireDate
        {
            get
            {
                return _isExpireDate;
            }
            set
            {

                if (_isExpireDate == value)
                    return;
                
                SetProperty(ref _isExpireDate, value);
            }
        }

        private DateTime _expireDate = DateTime.Now.AddYears(2);
        [ProtoMember(2)]
        public DateTime ExpireDate
        {
            get
            {
                return _expireDate;
            }
            set
            {
                SetProperty(ref _expireDate, value);
            }
        }

        private bool _isReaddCount;
        [ProtoMember(3)]
        public bool IsReaddCount
        {
            get
            {
                return _isReaddCount;
            }
            set
            {
                if (_isReaddCount == value)
                    return;
               
                SetProperty(ref _isReaddCount, value);
            }
        }

        private int _readdCount;
        [ProtoMember(4)]
        public int ReaddCount
        {
            get
            {
                return _readdCount;
            }
            set
            {
                if (_readdCount == value)
                    return;
               
                SetProperty(ref _readdCount, value);
            }
        }
    }
}
