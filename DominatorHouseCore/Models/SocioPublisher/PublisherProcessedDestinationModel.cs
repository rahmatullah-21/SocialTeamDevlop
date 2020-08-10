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
                SetProperty(ref _campaignId, value);
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
                SetProperty(ref _listTotalDestination, value);
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
                SetProperty(ref _listProcessedDestination, value);
            }
        }

        private int _destinationCount;

        [ProtoMember(4)]
        public int DestinationCount
        {
            get
            {
                return _destinationCount;
            }
            set
            {
                if (_destinationCount == value)
                    return;
                SetProperty(ref _destinationCount, value);
            }
        }

        private List<PostDestinationModel> _listSkippedDestination = new List<PostDestinationModel>();

        [ProtoMember(5)]
        public List<PostDestinationModel> ListSkippedDestination
        {
            get
            {
                return _listSkippedDestination;
            }
            set
            {
                if (_listSkippedDestination == value)
                    return;
                SetProperty(ref _listSkippedDestination, value);
            }
        }
    }



    [Serializable]
    [ProtoContract]
    public class PostDestinationModel : BindableBase
    {
        private List<string> _destinationGuidList;

        [ProtoMember(1)]
        public List<string> DestinationGuidList
        {
            get
            {
                return _destinationGuidList;
            }
            set
            {
                if (_destinationGuidList == value)
                    return;
                SetProperty(ref _destinationGuidList, value);
            }
        }

    }
}
