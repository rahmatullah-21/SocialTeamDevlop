using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Settings
{
    [ProtoContract]
    public class BlacklistSettings:BindableBase
    {
        private bool _addInteractedBlacklist;

        [ProtoMember(1)]
        public bool AddInteractedBlacklist
        {
            get
            {
                return _addInteractedBlacklist;
            }
            set
            {
                if (value == _addInteractedBlacklist) return; SetProperty(ref _addInteractedBlacklist, value); }
        }
        private bool _pilterBlacklist;

       [ProtoMember(2)]
        public bool FilterBlacklist {
            get
            {
                return _pilterBlacklist;
            }
            set
            {
                if (value == _pilterBlacklist)
                    return;
                SetProperty(ref _pilterBlacklist, value);
            }
        }

    }

}