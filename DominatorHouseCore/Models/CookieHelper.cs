using DominatorHouseCore.Utility;
using ProtoBuf;
using System;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class CookieHelper : BindableBase
    {
        [ProtoMember(3)]
        public string Domain { get; set; }

        private string _name = string.Empty;
        [ProtoMember(1)]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;
                SetProperty(ref _name, value);
            }
        }

        private string _value = string.Empty;
        [ProtoMember(2)]
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                SetProperty(ref _value, value);
            }
        }
        private DateTime _Expires = new DateTime();
        [ProtoMember(4)]
        public DateTime Expires
        {
            get { return _Expires; }
            set
            {
                if (_Expires == value)
                    return;
                SetProperty(ref _Expires, value);
            }
        }
    }
}
