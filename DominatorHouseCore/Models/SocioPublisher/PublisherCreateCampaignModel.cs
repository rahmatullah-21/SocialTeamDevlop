using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherCreateCampaignModel : BindableBase
    {
        public PublisherCreateCampaignModel()
        {
            //_campaignList.Add(_campaignName);
            CampaignId = Utilities.GetGuid(true);
        }
        [ProtoMember(1)]
        public string CampaignId { get; set; }

        private string _campaignName=$"Campaign {DateTimeUtilities.GetEpochTime()}";
        // To specify the campaign name
        [ProtoMember(2)]
        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                if (_campaignName == value)
                    return;
                _campaignName = value;
                OnPropertyChanged(nameof(CampaignName));
            }
        }

        private string _campaignStatus= "Active";
        // To specify the campaign status
        [ProtoMember(3)]
        public string CampaignStatus
        {
            get
            {
                return _campaignStatus;
            }
            set
            {
                if (_campaignStatus == value)
                    return;
                _campaignStatus = value;
                OnPropertyChanged(nameof(CampaignStatus));
            }
        }


        //private ObservableCollection<string> _campaignList = new ObservableCollection<string>();
        //// To hold all available the campaign name
        ////[ProtoMember(4)]
        //public ObservableCollection<string> CampaignList
        //{
        //    get
        //    {
        //        return _campaignList;
        //    }
        //    set
        //    {
        //        if (_campaignList == value)
        //            return;
        //        _campaignList = value;
        //        OnPropertyChanged(nameof(CampaignList));
        //    }
        //}
        [ProtoMember(5)]
        public JobConfigurationModel JobConfigurations { get; set; } = new JobConfigurationModel();
        [ProtoMember(6)]
        public OtherConfigurationModel OtherConfiguration { get; set; } = new OtherConfigurationModel();
        [ProtoMember(7)]
        public PostDetailsModel PostDetailsModel { get; set; }=new PostDetailsModel();
        private ObservableCollection<string> _lstDestinationId = new ObservableCollection<string>();
        
        [ProtoMember(8)]
        public ObservableCollection<string> LstDestinationId
        {
            get
            {
                return _lstDestinationId;
            }
            set
            {
                if (_lstDestinationId == value)
                    return;
                _lstDestinationId = value;
                OnPropertyChanged(nameof(_lstDestinationId));
            }
        }

    }
}