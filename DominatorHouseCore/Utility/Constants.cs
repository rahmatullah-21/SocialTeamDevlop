using System;
using System.ComponentModel;
using System.Reflection;

namespace DominatorHouseCore.Utility
{
    [Localizable(false)]
    public static class Constants
    {
      
        public static string ApiUrl => $"{(object)INSTAGRAM_BASE_URL}api/v1/";

        public static string ApplicationName => "GramDominator 3.0";

        public static string ContentTypeDefault => "application/x-www-form-urlencoded; charset=UTF-8";

      
     
        public static int FloodWait => 60000;

        public static string IG_SIG_KEY
        {
            get
            {
                return "9cf2daac9a7802e9d199ad5dc6e221fe8da535359b58b60a1265a281b2a30a83";
            }
        }

        public static string IG_VERSION
        {
            get
            {
                return "40.0.0.14.95";
            }
        }

        public static string INSTAGRAM_BASE_URL
        {
            get
            {
                return "https://i.instagram.com/";
            }
        }

     
        public static string SIG_KEY_VERSION
        {
            get
            {
                return "4";
            }
        }

        public static string STATUS_OK_RESPONSE
        {
            get
            {
                return "{\"status\": \"ok\"}";
            }
        }

     

      
      
        public static string USERAGENT_FORMAT
        {
            get
            {
                return "Instagram {0} Android ({1}/{2}; {3}; {4}; {5}; {6}; {7}; {8}; {9})";
            }
        }

        public static string USERAGENT_LOCALE
        {
            get
            {
                return "en_US";
            }
        }

     

        public static string X_FB_HTTP_Engine
        {
            get
            {
                return "Liger";
            }
        }

        public static string X_IG_Capabilities
        {
            get
            {
                return "3boDAA==";
            }
        }

        public static string X_IG_Connection_Type
        {
            get
            {
                return "WIFI";
            }
        }

        public static string Referer
        {
            get
            {
                return "https://i.instagram.com";
            }
        }


        public static string Origin
        {
            get
            {
                return "https://i.instagram.com";
            }
        }

        public static string USER_AGENT_LOCALE
        {
            get
            {
                return "en_US";
            }
        }


        public static string ACCEPT_LANGUAGE
        {
            get
            {
                return "en_US";
            }
        }

        //

        public static string ACCEPT_ENCODING
        {
            get
            {
                return "gzip,deflate";
            }
        }

        

        public static int AppVersion
        {
            get
            {
                try
                {
                    string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    return Int32.Parse(version.Substring(version.LastIndexOf(".") + 1));
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
                return 0;
            }
        }

        public static string ProductName = "Socinator";
        public static string ClickOnceFileName = "Socinator.appref-ms";


    }
}
