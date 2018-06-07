using System;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class LdPostSettings : BindableBase, ILdPostSettings
    {
        private LdGroupPostType _groupPostType = LdGroupPostType.General;

        [ProtoMember(1)]
        public LdGroupPostType GroupPostType
        {
            get
            {
                return _groupPostType;
            }
            set
            {
                if(_groupPostType == value)
                    return;
                _groupPostType = value;
                SetProperty(ref _groupPostType, value);
            }
        }
    }
}