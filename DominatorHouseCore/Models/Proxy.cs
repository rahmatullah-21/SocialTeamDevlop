using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Net;
using System.Text.RegularExpressions;

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

        [ProtoMember(10)]
        public string ProxyId { get; set; }
        [ProtoMember(5)]
        public bool HasCredentials { get; private set; }


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



        public void SetProxyCredentials(string proxyUsername, string proxyPassword)
        {
            ProxyUsername = proxyUsername;
            ProxyPassword = proxyPassword;
            HasCredentials = true;
        }

        public string GetProxy()
        {
            if (!string.IsNullOrWhiteSpace(ProxyIp))
                return ProxyIp + ":" + ProxyPort;

            return Resources.UserAccountEditPasswordNotValue;
        }


        // ReSharper disable once UnusedMember.Global
        public bool TestProxy()
        {
            if (ProxyIp == null && ProxyPort == null)
                throw new ArgumentException("Need to set proxies first");
            try
            {
                var webClientExtended = new WebClientExtended();
                var webProxy1 = new WebProxy(GetProxy(), true);
                if (HasCredentials)
                    webProxy1.Credentials = new NetworkCredential(ProxyUsername, ProxyPassword);
                var webProxy2 = webProxy1;
                webClientExtended.Proxy = webProxy2;
                var address = "http://google.com/";
                webClientExtended.DownloadString(address);
                return true;
            }
            catch
            {
                return false;
            }
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

        private class WebClientExtended : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                var webRequest = (HttpWebRequest)base.GetWebRequest(uri);
                if (webRequest == null)
                    throw new ArgumentException();
                int num1 = 5000;
                webRequest.Timeout = num1;
                int num2 = 0;
                webRequest.KeepAlive = num2 != 0;
                webRequest.ServicePoint.SetTcpKeepAlive(false, 1000, 5000);
                return webRequest;
            }
        }


        //[return: TupleElementNames(new string[] { "username", "password" })]
        //public ValueTuple<string, string> GetCredentials()
        //{
        //    return new ValueTuple<string, string>(this.Pusername, this.Ppassword);
        //}

    }
}
