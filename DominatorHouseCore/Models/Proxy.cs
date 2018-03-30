using System;
using System.Net;
using System.Text.RegularExpressions;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Runtime.CompilerServices;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class Proxy : BindableBase
    {
        private string _proxyIp;
        private string _proxyPort;
        private string _proxyUsername;
        private string _proxyPassword;
      

        public Proxy()
        {
            this.HasCredentials = false;
            this.HasProxy = false;
        }


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

        public void ChangeProxy(string proxyIp, string proxyPort)
        {
            if (string.IsNullOrWhiteSpace(proxyIp) || string.IsNullOrWhiteSpace(proxyPort))
                throw new ArgumentException("Invalid proxy format.");
            if (!Proxy.IsValidProxy(proxyIp, proxyPort))
                throw new ArgumentException("Invalid proxy format.");
            this.ProxyIp = proxyIp;
            this.ProxyPort = proxyPort;
            this.HasProxy = true;
            this.SetProxyCredentials();
        }



        public void ChangeProxy()
        {
            this.ProxyIp = (string)null;
            this.ProxyPort = (string)null;
            this.HasProxy = false;
            this.SetProxyCredentials();
        }



        public void SetProxyCredentials(string proxyUsername, string proxyPassword)
        {
            this.ProxyUsername = proxyUsername;
            this.ProxyPassword = proxyPassword;
            this.HasCredentials = true;
        }



        private void SetProxyCredentials()
        {
            this.ProxyUsername = (string)null;
            this.ProxyPassword = (string)null;
            this.HasCredentials = false;
        }



        public string GetProxy()
        {
            if (!string.IsNullOrWhiteSpace(this.ProxyIp))
                return this.ProxyIp + ":" + this.ProxyPort;

            return Resources.UserAccountEditPasswordNotValue;
        }


        public bool TestProxy()
        {
            if (this.ProxyIp == null && this.ProxyPort == null)
                throw new ArgumentException("Need to set proxies first");
            try
            {
                var webClientExtended = new Proxy.WebClientExtended();
                var webProxy1 = new WebProxy(this.GetProxy(), true);
                if (this.HasCredentials)
                    webProxy1.Credentials = (ICredentials)new NetworkCredential(this.ProxyUsername, this.ProxyPassword);
                var webProxy2 = webProxy1;
                webClientExtended.Proxy = (IWebProxy)webProxy2;
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
            return Regex.IsMatch(proxyAddress, "^([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])\\.([01]?\\d\\d?|2[0-4]\\d|25[0-5])$");

        }

        public static bool IsValidProxyPort(string proxyPort)
        {

            //  return Regex.IsMatch(proxyPort, "^\\d{1,5}$");
             return Regex.IsMatch(proxyPort, "^([0-9]{1,4}$|[1-5][0-9]{4}$|6[0-4][0-9]{3}$|65[0-4][0-9]{2}$|655[0-2][0-9]$|6553[0-5])$");
          //  return (int.Parse(proxyPort) >= 0 && int.Parse(proxyPort) <= 65535);
        }


        // [return: TupleElementNames(new string[] { "username", "password" })]
        public ValueTuple<string, string> GetCredentials()
        {
            return new ValueTuple<string, string>(this.ProxyUsername, this.ProxyPassword);
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
                return (WebRequest)webRequest;
            }
        }


        //[return: TupleElementNames(new string[] { "username", "password" })]
        //public ValueTuple<string, string> GetCredentials()
        //{
        //    return new ValueTuple<string, string>(this.Pusername, this.Ppassword);
        //}

    }
}
