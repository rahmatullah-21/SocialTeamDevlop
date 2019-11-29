using System;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class PublisherPostSettings :BindableBase
    {
        private GeneralPostSettings _generalPostSettings = new GeneralPostSettings();
        private GdPostSettings _gdPostSettings = new GdPostSettings();

        [ProtoMember(1)]
        public GeneralPostSettings GeneralPostSettings
        {
            get
            {
                return _generalPostSettings;
            }
            set
            {               
                if (_generalPostSettings == value)
                    return;

                SetProperty(ref _generalPostSettings, value);
            }
        }
        [ProtoMember(3)]
        public GdPostSettings GdPostSettings
        {
            get { return _gdPostSettings; }
            set { SetProperty(ref _gdPostSettings, value); }
        }

    }
}