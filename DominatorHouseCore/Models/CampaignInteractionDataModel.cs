using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{

    [ProtoContract]
    public class CampaignInteractionDataModel
    {
        [ProtoMember(1)]
        public string CampaignId { get; set; } = string.Empty;

        [ProtoMember(2)]
        public SortedList<string, DateTime> InteractedData { get; set; } = new SortedList<string, DateTime>();

       

    }
}
