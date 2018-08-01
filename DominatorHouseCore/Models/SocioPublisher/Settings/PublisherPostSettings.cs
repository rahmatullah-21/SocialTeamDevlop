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

        [ProtoMember(2)]
        public FdPostSettings FdPostSettings { get; set; }=new FdPostSettings();

        [ProtoMember(3)]
        public GdPostSettings GdPostSettings { get; set; }=new GdPostSettings();

        [ProtoMember(4)]
        public TdPostSettings TdPostSettings { get; set; }=new TdPostSettings();

        [ProtoMember(5)]
        public LdPostSettings LdPostSettings { get; set; }=new LdPostSettings();

        [ProtoMember(6)]
        public TumberPostSettings TumberPostSettings { get; set; }=new TumberPostSettings();

        [ProtoMember(7)]
        public RedditPostSetting RedditPostSetting { get; set; } = new RedditPostSetting();
    }
}