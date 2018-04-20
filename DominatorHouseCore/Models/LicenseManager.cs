using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class LicenseManager
    {      
        [ProtoMember(1)]  
        public string LicenseKey { get; set; }

        [ProtoMember(2)]
        public DateTime LicenseAddedDate { get; set; }

        [ProtoMember(3)]
        public string LicensedAuthor { get; set; }

        [ProtoMember(4)]
        public HashSet<SocialNetworks> LicensedNetworks { get; set; }
    }
}