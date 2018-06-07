using System;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class TumberPostSettings : BindableBase, ITumblrSettings
    {
        private bool _isTagUser;
        private string _tagUserList = string.Empty;

        [ProtoMember(1)]
        public bool IsTagUser
        {
            get
            {
                return _isTagUser;
            }
            set
            {              
                if (_isTagUser == value)
                    return;

                _isTagUser = value;
                SetProperty(ref _isTagUser, value);
            }
        }

        [ProtoMember(2)]
        public string TagUserList
        {
            get
            {
                return _tagUserList;
            }
            set
            {              
                if (_tagUserList == value)
                    return;

                _tagUserList = value;
                SetProperty(ref _tagUserList, value);
            }
        }
    }
}