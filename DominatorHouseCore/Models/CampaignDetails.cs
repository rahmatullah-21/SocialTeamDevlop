using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    /// <summary>
    /// Contains module/submodule name, selected accounts
    /// Purpose of campaign is to share it between several users
    /// </summary>
    [ProtoContract]
    public class CampaignDetails : BindableBase
    {
        public CampaignDetails()
        {
            CampaignId = Utilities.GetGuid(true);
        }

        [ProtoMember(10)]
        public string CampaignId { get; }


        [ProtoMember(1)]
        public string CampaignName { get; set; }


        [ProtoMember(2)]
        public string MainModule { get; set; }


        [ProtoMember(3)]
        public string SubModule { get; set; }


        [ProtoMember(4)]
        public SocialNetworks SocialNetworks { get; set; }

        private List<string> _selectedAccountList = new List<string>();
        [ProtoMember(5)]
        public List<string> SelectedAccountList
        {
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

        private ICollectionView _campaignCollection;
        public ICollectionView CampaignCollection
        {
            get
            {
                return _campaignCollection;
            }
            set
            {
                if (_campaignCollection != null && _campaignCollection == value)
                    return;
                SetProperty(ref _campaignCollection, value);
            }
        }
        private ObservableCollection<CampaignDetails> _campaignDetails = new ObservableCollection<CampaignDetails>();

        public ObservableCollection<CampaignDetails> ObjCampaignDetails
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

        private bool _isCampaignChecked;

        public bool IsCampaignChecked
        {
            get
            {
                return _isCampaignChecked;
            }
            set
            {
                if (_isCampaignChecked == value)
                    return;
                SetProperty(ref _isCampaignChecked, value);
            }
        }
        private bool _isAllCampaignChecked;

        public bool IsAllCampaignChecked
        {
            get
            {
                return _isAllCampaignChecked;
            }
            set
            {
                if (_isAllCampaignChecked == value)
                    return;
                SetProperty(ref _isAllCampaignChecked, value);
            }
        }
        private ObservableCollection<string> _activityType=new ObservableCollection<string>();

        public ObservableCollection<string> ActivityType
        {
            get
            {
                return _activityType;
            }
            set
            {
                if (_activityType == value)
                    return;
                SetProperty(ref _activityType, value);
            }
        }
        private string _selectedActivity=string.Empty;

        public string SelectedActivity
        {
            get
            {
                return _selectedActivity;
            }
            set
            {
                if (_selectedActivity == value)
                    return;
                SetProperty(ref _selectedActivity, value);
            }
        }
    }



}