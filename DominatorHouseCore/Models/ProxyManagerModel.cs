using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ProxyManagerModel : BindableBase
    {
       
        private ICollectionView _proxyManagerCollection ;
        public ICollectionView ProxyManagerCollection
        {
            get
            {
                return _proxyManagerCollection;
            }
            set
            {
                if (_proxyManagerCollection != null && _proxyManagerCollection == value)
                    return;
                SetProperty(ref _proxyManagerCollection, value);
            }
        }

        private Proxy _accountProxy = new Proxy();
        [ProtoMember(1)]
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

        private bool _isHideAssignedSocialProfiles;
        [ProtoMember(2)]
        public bool IsHideAssignedSocialProfiles
        {
            get
            {
                return _isHideAssignedSocialProfiles;
            }
            set
            {
                if (_isHideAssignedSocialProfiles == value)
                    return;
                SetProperty(ref _isHideAssignedSocialProfiles, value);
            }
        }

        private bool _isHideUsernameAndPassword;
        [ProtoMember(3)]
        public bool IsHideUsernameAndPasswordoperty
        {
            get
            {
                return _isHideUsernameAndPassword;
            }
            set
            {
                if (_isHideUsernameAndPassword == value)
                    return;
                SetProperty(ref _isHideUsernameAndPassword, value);
            }
        }
        private bool _isShowByGroup;
        [ProtoMember(4)]
        public bool IsShowByGroup
        {
            get
            {
                return _isShowByGroup;
            }
            set
            {
                if (_isShowByGroup == value)
                    return;
                SetProperty(ref _isShowByGroup, value);
            }
        }
        private bool _isFilterByGroup;
        [ProtoMember(5)]
        public bool FilterByGroup
        {
            get
            {
                return _isFilterByGroup;
            }
            set
            {
                if (_isFilterByGroup == value)
                    return;
                SetProperty(ref _isFilterByGroup, value);
            }
        }

        private bool _isShowProxiesWithError;
        [ProtoMember(6)]
        public bool IsShowProxiesWithError
        {
            get
            {
                return _isShowProxiesWithError;
            }
            set
            {
                if (_isShowProxiesWithError == value)
                    return;
                SetProperty(ref _isShowProxiesWithError, value);

            }
        }
        private bool _isShowUnassignedProxies;
        [ProtoMember(7)]
        public bool IsShowUnassignedProxies
        {
            get
            {
                return _isShowUnassignedProxies;
            }
            set
            {
                if (_isShowUnassignedProxies == value)
                    return;
                SetProperty(ref _isShowUnassignedProxies, value);
            }
        }
        private string _filter;
        [ProtoMember(8)]
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (_filter == value)
                    return;
                SetProperty(ref _filter, value);
            }
        }
        private string _uRLToUseToVerifyProxies;
        [ProtoMember(9)]
        public string URLToUseToVerifyProxies
        {
            get
            {
                return _uRLToUseToVerifyProxies;
            }
            set
            {
                if (_uRLToUseToVerifyProxies == value)
                    return;
                SetProperty(ref _uRLToUseToVerifyProxies, value);
            }
        }
        private ObservableCollection<string> _groups=new ObservableCollection<string>();

        public ObservableCollection<string> Groups
        {
            get
            {
                return _groups;
            }
            set
            {
                if (_groups == value)
                    return;
                SetProperty(ref _groups, value);
            }
        }
        private int _id;
        [ProtoMember(10)]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id == value)
                    return;
                SetProperty(ref _id, value);
            }
        }
        private bool _isProxySelected;
        [ProtoMember(11)]
        public bool IsProxySelected
        {
            get
            {
                return _isProxySelected;
            }
            set
            {
                if (_isProxySelected == value)
                    return;
                SetProperty(ref _isProxySelected, value);
            }
        }
        private string _status="Not Checked";
        [ProtoMember(12)]
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
        private string _responseTime="0 milli seconds";
        [ProtoMember(13)]
        public string ResponseTime
        {
            get
            {
                return _responseTime;
            }
            set
            {
                if (_responseTime == value)
                    return;
                SetProperty(ref _responseTime, value);
            }
        }
        private int _failures;
        [ProtoMember(14)]
        public int Failures
        {
            get
            {
                return _failures;
            }
            set
            {
                if (_failures == value)
                    return;
                SetProperty(ref _failures, value);
            }
        }
        private ObservableCollection<AccountAssign> _accountsAssignedto=new ObservableCollection<AccountAssign>();
        [ProtoMember(15)]
        public ObservableCollection<AccountAssign> AccountsAssignedto
        {
            get
            {
                return _accountsAssignedto;
            }
            set
            {
                if (_accountsAssignedto == value)
                    return;
                SetProperty(ref _accountsAssignedto, value);
            }
        }
        private ObservableCollection<AccountAssign> _accountsToBeAssign = new ObservableCollection<AccountAssign>();
       
        public ObservableCollection<AccountAssign> AccountsToBeAssign
        {
            get
            {
                return _accountsToBeAssign;
            }
            set
            {
                if (_accountsToBeAssign == value)
                    return;
                SetProperty(ref _accountsToBeAssign, value);
            }
        }
        private string _group ;
        [ProtoMember(16)]
        public string Group
        {
            get
            {
                return _group;
            }
            set
            {
                if (_group == value)
                    return;
                SetProperty(ref _group, value);
            }
        }

    }
    [ProtoContract]
    public class AccountAssign : BindableBase
    {
        private string _userName;
        [ProtoMember(1)]
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName == value)
                    return;
                SetProperty(ref _userName, value);
            }
        }
        private SocialNetworks _accountNetwork = SocialNetworks.Facebook;

        [ProtoMember(2)]
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

            }
        }
    }
}
