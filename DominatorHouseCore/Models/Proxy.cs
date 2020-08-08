using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Net;
using System.Text.RegularExpressions;
using CommonServiceLocator;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class Proxy : BindableBase
    {
        private static readonly Regex ProxyIpValidationRegex = new Regex("^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])$");
        private static readonly Regex ProxyPortValidationRegex = new Regex("^([0-9]{1,4}$|[1-5][0-9]{4}$|6[0-4][0-9]{3}$|65[0-4][0-9]{2}$|655[0-2][0-9]$|6553[0-5])$");
        private string _proxyIp;
        private string _proxyPort;
        private string _proxyUsername;
        private string _proxyPassword;

        public Proxy()
        {
            HasCredentials = false;
            HasProxy = false;
            ProxyId = Utilities.GetGuid();
        }

        #region Properties

        [ProtoMember(10)]
        public string ProxyId { get; set; }
        [ProtoMember(5)]
        public bool HasCredentials { get; set; }

        [ProtoMember(6)]
        public bool HasProxy { get; private set; }

        [ProtoMember(1)]
        public string ProxyIp
        {
            get
            {
                return _proxyIp;
            }
            set
            {
                if (_proxyIp != null && value == _proxyIp)
                    return;
                SetProperty(ref _proxyIp, value);
            }
        }

        [ProtoMember(2)]
        public string ProxyPort
        {
            get
            {
                return _proxyPort;
            }
            set
            {
                if (_proxyPort != null && value == _proxyPort)
                    return;
                SetProperty(ref _proxyPort, value);
            }
        }

        [ProtoMember(4)]
        public string ProxyPassword
        {
            get
            {
                return _proxyPassword;
            }
            set
            {
                if (_proxyPassword != null && value == _proxyPassword)
                    return;
                SetProperty(ref _proxyPassword, value);
            }
        }

        [ProtoMember(3)]
        public string ProxyUsername
        {
            get
            {
                return _proxyUsername;
            }
            set
            {
                if (_proxyUsername != null && value == _proxyUsername)
                    return;
                SetProperty(ref _proxyUsername, value);
            }
        }

        private string _proxyName;

        [ProtoMember(7)]
        public string ProxyName
        {
            get
            {
                return _proxyName;
            }
            set
            {
                if (_proxyName != null && value == _proxyName)
                    return;
                SetProperty(ref _proxyName, value);
            }
        }

        private string _proxyGroup = "UnGroup";
        [ProtoMember(8)]
        public string ProxyGroup
        {
            get
            {
                return _proxyGroup;
            }
            set
            {
                if (_proxyGroup == value)
                    return;
                SetProperty(ref _proxyGroup, value);
            }
        }

        #endregion

        #region Methods

        public string GetProxy()
        {
            if (!string.IsNullOrWhiteSpace(ProxyIp))
                return ProxyIp + ":" + ProxyPort;

            return Resources.UserAccountEditPasswordNotValue;
        }
        public static bool IsValidProxy(string ip, string port)
        {
            return Regex.IsMatch(ip + ":" + port, "^\\d{1,3}(\\.\\d{1,3}){3}:\\d{1,5}$");
        }
        public static bool IsValidProxyIp(string proxyAddress)
        {
            return ProxyIpValidationRegex.IsMatch(proxyAddress) || IsLuminatiProxy(proxyAddress);
        }
        public static bool IsLuminatiProxy(string proxyAddress)
        {
            return proxyAddress.Contains("zproxy.lum-superproxy.io");
        }
        public static bool IsValidProxyPort(string proxyPort)
        {
            return ProxyPortValidationRegex.IsMatch(proxyPort);
        } 

        #endregion

    }
}
