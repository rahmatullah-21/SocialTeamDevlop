using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class CampaignDetails:BindableBase
    {
        public CampaignDetails()
        {
            CampaignId = Utilities.GetGuid(true);
        }

        [ProtoMember(10)]
        public string CampaignId { get; set; }


        [ProtoMember(1)]
        public string CampaignName { get; set; }


        [ProtoMember(2)]
        public string MainModule { get; set; }


        [ProtoMember(3)]
        public string SubModule { get; set; }


        [ProtoMember(4)]
        public SocialNetworks SocialNetworks { get; set; }

        private List<string> _selectedAccountList = new List<string>();
        [ProtoMember(5)] public List<string> SelectedAccountList {
            get
            {
                return _selectedAccountList;
            }
            set
            {
                if (_campaignDetails != null && _selectedAccountList == value)
                    return;
                SetProperty(ref _selectedAccountList, value);
            }
        } 


        [ProtoMember(6)]
        public string TemplateId { get; set; }


        [ProtoMember(7)]
        public int CreationDate { get; set; }


        [ProtoMember(8)]
        public string Status { get; set; }


        [ProtoMember(9)]
        public int LastEditedDate { get; set; }
       

        private ObservableCollectionBase<CampaignDetails> _campaignDetails=new ObservableCollectionBase<CampaignDetails>();
       
        public ObservableCollectionBase<CampaignDetails> ObjCampaignDetails
        {
            get
            {
                return _campaignDetails;
            }
            set
            {
                if (_campaignDetails != null && _campaignDetails == value)
                  return;
                SetProperty(ref _campaignDetails, value);
            }
        }
        public bool SaveCampaign(CampaignDetails campaignDetails, SocialNetworks socialNetworks)
        {
            try
            {
                // Get the path for current social network
                var socialNetworkPath = new SocialNetworkPathFactory(socialNetworks).GetSocialNetworkCampaignPath();

                DirectoryUtilities.CreateDirectory(socialNetworkPath);

                // Serialize the template configuration to bin files
                ProtoBuffBase.SerializeObjects(campaignDetails, $"{socialNetworkPath}//{ConstantVariable.CampaignDetails}");
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

    }
}