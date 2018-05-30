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

        private HashSet<string> _groups=new HashSet<string>();

        public HashSet<string> Groups
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

        private bool _isProxySelected;
    
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
        [ProtoMember(16)]
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
        [ProtoMember(17)]
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
