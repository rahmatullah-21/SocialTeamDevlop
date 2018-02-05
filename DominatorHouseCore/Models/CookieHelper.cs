using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class CookieHelper
    {
        [ProtoMember(3)]
        public string Domain { get; set; }

        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string Value { get; set; }
    }
}
