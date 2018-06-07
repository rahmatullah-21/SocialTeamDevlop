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
            _campaignList.Add("Item1");
            _campaignList.Add("Item2");
            _campaignList.Add("Item3");
        }

        private string _campaignName;
        // To specify the campaign name
        [ProtoMember(1)]
        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                if(_campaignName==value)
                    return;
                _campaignName = value;
                OnPropertyChanged(nameof(CampaignName));
            }
        }

        private string _campaignStatus;
        // To specify the campaign status
        [ProtoMember(2)]
        public string CampaignStatus
        {
            get
            {
                return _campaignStatus;
            }
            set
            {
                if(_campaignStatus==value)
                    return;
                _campaignStatus = value;
                OnPropertyChanged(nameof(CampaignStatus));
            }
        }


        private ObservableCollection<string> _campaignList = new ObservableCollection<string>();
        // To hold all available the campaign name
        [ProtoMember(3)]
        public ObservableCollection<string> CampaignList
        {
            get
            {
                return _campaignList;
            }
            set
            {
                if(_campaignList == value)
                    return;
                _campaignList = value;
                OnPropertyChanged(nameof(CampaignList));
            }
        }
        [ProtoMember(4)]
       public JobConfigurationModel JobConfigurations { get; set; } = new JobConfigurationModel();
        [ProtoMember(5)]
        public OtherConfigurationModel OtherConfiguration { get; set; } = new OtherConfigurationModel();
    }
}