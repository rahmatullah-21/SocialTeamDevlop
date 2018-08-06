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
        public string CampaignId { get; set; }


        [ProtoMember(1)]
        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                if (_campaignName != null && _campaignName == value)
                    return;
                SetProperty(ref _campaignName, value);
            }
        }


        [ProtoMember(2)]
        public string MainModule
        {
            get
            {
                return _mainModule;
            }
            set
            {
                if (_mainModule != null && _mainModule == value)
                    return;
                SetProperty(ref _mainModule, value);
            }
        }


        [ProtoMember(3)]
        public string SubModule
        {
            get
            {
                return _subModule;
            }
            set
            {
                if (_subModule != null && _subModule == value)
                    return;
                SetProperty(ref _subModule, value);
            }
        }


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
        public string TemplateId
        {
            get
            {
                return _templateId;
            }
            set
            {
                if (_templateId != null && _templateId == value)
                    return;
                SetProperty(ref _templateId, value);
            }
        }


        [ProtoMember(7)]
        public int CreationDate
        {
            get
            {
                return _creationDate;
            }
            set
            {
                if (_creationDate == value)
                    return;
                SetProperty(ref _creationDate, value);
            }
        }


        [ProtoMember(8)]
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status == value)
                    return;
                SetProperty(ref _status, value);
            }
        }


        [ProtoMember(9)]
        public int LastEditedDate
        {
            get
            {
                return _lastEditedDate;
            }
            set
            {
                if (_lastEditedDate == value)
                    return;
                SetProperty(ref _lastEditedDate, value);
            }
        }

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
        private ObservableCollection<string> _activityType = new ObservableCollection<string>();

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
        private string _selectedActivity = string.Empty;
        private string _campaignName;
        private string _mainModule;
        private string _subModule;
        private string _templateId;
        private int _creationDate;
        private string _status;
        private int _lastEditedDate;

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