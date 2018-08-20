using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class FatalErrorHandler
    {      
        [ProtoMember(1)]  
        public string FatalErrorMessage { get; set; }

        [ProtoMember(2)]
        public DateTime FatalErrorAddedDate { get; set; }

        [ProtoMember(3)]
        public string ErrorSource { get; set; }

        [ProtoMember(4)]
        public HashSet<SocialNetworks> ErrorNetworks { get; set; }
    }
}