using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class QuoraModel : BindableBase
    {
        private bool _isEnableFollowDifferentUserChecked;
        [ProtoMember(1)]
        public bool IsEnableFollowDifferentUserChecked
        {
            get
            {
                return _isEnableFollowDifferentUserChecked;
            }
            set
            {
                if (value == _isEnableFollowDifferentUserChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUserChecked, value);
            }
        }
    }
    [ProtoContract]
    public class LinkedInModel : BindableBase
    {
        private bool _isEnableExportingHTMLOfDifferentConnections;
        [ProtoMember(1)]
        public bool IsEnableExportingHTMLOfDifferentConnections
        {
            get
            {
                return _isEnableExportingHTMLOfDifferentConnections;
            }
            set
            {
                if (value == _isEnableExportingHTMLOfDifferentConnections)
                    return;
                SetProperty(ref _isEnableExportingHTMLOfDifferentConnections, value);
            }
        }
    }
}