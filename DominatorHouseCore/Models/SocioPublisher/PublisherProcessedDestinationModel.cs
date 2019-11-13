using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublisherProcessedDestinationModel : BindableBase
    {
        private string _campaignId;

        [ProtoMember(1)]
        public string CampaignId
        {
            get
            {
                return _campaignId;
            }
            set
            {
                if (_campaignId == value)
                    return;
                _campaignId = value;
                OnPropertyChanged(nameof(CampaignId));
            }
        }

        private List<string> _listTotalDestination = new List<string>();
        
        [ProtoMember(2)]
        public List<string> ListTotalDestination
        {
            get
            {
                return _listTotalDestination;
            }
            set
            {
                if (_listTotalDestination == value)
                    return;
                _listTotalDestination = value;
                OnPropertyChanged(nameof(ListTotalDestination));
            }
        }

        private List<string> _listProcessedDestination = new List<string>();

        [ProtoMember(3)]
        public List<string> ListProcessedDestination
        {
            get
            {
                return _listProcessedDestination;
            }
            set
            {
                if (_listProcessedDestination == value)
                    return;
                _listProcessedDestination = value;
                OnPropertyChanged(nameof(ListProcessedDestination));
            }
        }
    }
}
