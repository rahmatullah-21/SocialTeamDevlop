using System.Collections.Generic;
using DominatorHouseCore.Models;
using ProtoBuf;

namespace DominatorHouseCore.ViewModel
{
    [ProtoContract]
    public class GlobalInteractionViewModel
    {
        [ProtoMember(1)]
        public Dictionary<string, GlobalInteractionDataModel> GlobalInteractedCollections { get; set; } = new Dictionary<string, GlobalInteractionDataModel>();
    }
}