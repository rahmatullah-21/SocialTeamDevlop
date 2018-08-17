using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using ProtoBuf;
using System.Collections.Generic;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class DominatorAccountBaseModel : BindableBase
    {

        private SocialNetworks _accountNetwork = SocialNetworks.Facebook;
        /// <summary>
        ///  To Identify Account is belongs to which network
        /// </summary>
        [ProtoMember(1)]
        public SocialNetworks AccountNetwork
        {
            get
            {
                return _accountNetwork;
            }
            set
            {
                if (_accountNetwork == value)
                    return;
                SetProperty(ref _accountNetwork, value);
                SetGrowthProperties(_accountNetwork);
            }
        }
        [ProtoIgnore]
        public List<GrowthProperty> GrowthProperties { get; set; }

        private void SetGrowthProperties(SocialNetworks accountNetwork)
        {
            this.GrowthProperties = new List<GrowthProperty>();

            if (accountNetwork == SocialNetworks.Twitter)
            {

                this.GrowthProperties.Add(new GrowthProperty { PropertyName = "Followers", PropertyValue = 0 });
                this.GrowthProperties.Add(new GrowthProperty { PropertyName = "Followings", PropertyValue = 0 });
                this.GrowthProperties.Add(new GrowthProperty { PropertyName = "Tweets", PropertyValue = 0 });
            }
            else if (accountNetwork == SocialNetworks.Facebook)
            {
                this.GrowthProperties.Add(new GrowthProperty { PropertyName = "Friends", PropertyValue = 0 });
                this.GrowthProperties.Add(new GrowthProperty { PropertyName = "Followers", PropertyValue = 0 });
                this.GrowthProperties.Add(new GrowthProperty { PropertyName = "Shares", PropertyValue = 0 });
            }
        }
        private ContentSelectGroup _accountGroup = new ContentSelectGroup();
        /// <summary>
        /// To define the account is belongs to which group
        /// </summary>
        [ProtoMember(2)]
        public ContentSelectGroup AccountGroup
        {
            get
            {
                return _accountGroup;
            }
            set
            {
                if (_accountGroup == value)
                    return;
                SetProperty(ref _accountGroup, value);
            }
        }


        private string _userName = string.Empty;
        /// <summary>
        /// To define the social networks username of the accounts
        /// </summary>
        [ProtoMember(3)]
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != null && _userName == value)
                    return;
                SetProperty(ref _userName, value);
            }
        }

        private string _password = string.Empty;
        /// <summary>
        /// To store the account password
        /// </summary>
        [ProtoMember(4)]
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != null && _password == value)
                    return;
                SetProperty(ref _password, value);
            }
        }


        private string _userId = string.Empty;
        /// <summary>
        /// To define the social networks id of the account
        /// </summary>
        [ProtoMember(5)]
        public string UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                if (_userId != null && _userId == value)
                    return;
                SetProperty(ref _userId, value);
            }
        }


        private string _userFullName = string.Empty;
        /// <summary>
        /// To define the username of the account
        /// </summary>
        [ProtoMember(6)]
        public string UserFullName
        {
            get
            {
                return _userFullName;
            }
            set
            {
                if (_userFullName != null && _userFullName == value)
                    return;
                SetProperty(ref _userFullName, value);
            }
        }

        private string _profilePictureUrl = string.Empty;
        /// <summary>
        /// To define the profile picture url of the account
        /// </summary>
        [ProtoMember(7)]
        public string ProfilePictureUrl
        {
            get
            {
                return _profilePictureUrl;
            }
            set
            {
                if (_profilePictureUrl != null && _profilePictureUrl == value)
                    return;
                SetProperty(ref _profilePictureUrl, value);
            }
        }

        private Proxy _accountProxy = new Proxy();
        /// <summary>
        /// To define the Account Proxy
        /// </summary>       
        [ProtoMember(8)]
        public Proxy AccountProxy
        {
            get
            {
                return _accountProxy;
            }
            set
            {
                if (_accountProxy != null && _accountProxy == value)
                    return;
                SetProperty(ref _accountProxy, value);
            }
        }

        private string _accountId = Utilities.GetGuid(true);
        /// <summary>
        /// To access the account with unique Id
        /// </summary>
        [ProtoMember(9)]
        public string AccountId
        {
            get
            {
                return _accountId;
            }
            set
            {

                if (_accountId != null && _accountId == value)
                    return;
                SetProperty(ref _accountId, value);
            }
        }

        private AccountStatus _status;
        /// <summary>
        /// To define the status of the account
        /// </summary>
        [ProtoMember(10)]
        public AccountStatus Status
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
        private string _profileId;
        /// <summary>
        /// To define network profile username
        /// </summary>
        [ProtoMember(11)]
        public string ProfileId
        {
            get
            {
                return _profileId;
            }
            set
            {
                if (_profileId != null && _profileId == value)
                    return;
                SetProperty(ref _profileId, value);
            }
        }
        public override string ToString()
        {
            return string.Format("{0} on {1}", _userName, _accountNetwork);
        }


    }
}
