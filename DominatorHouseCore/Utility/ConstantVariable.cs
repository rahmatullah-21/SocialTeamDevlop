using System;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Utility
{
    public static class ConstantVariable
    {
        public static string UseragentCommonFormat { get; } = "Instagram {0} Android ({1}/{2}; {3}; {4}; {5}; {6}; {7}; {8}; {9})";

        public static string UseragentLocale { get; } = "en_US";

        public static string IgVersion { get; } = "10.33.0";

        public static string ApiUrl => $"{(object)ConstantVariable.InstagramBaseUrl}api/v1/";

        public static string InstagramBaseUrl { get; } = "https://i.instagram.com/";

        public static string XFbHttpEngine { get; } = "Liger";

        public static int WebrequestTimeout { get; } = 20000;

        public static bool UseSystemProxy { get; } = true;

        public static string ApplicationName { get; } = "Socinator";

        public static string GetPlatformBaseDirectory()
        {
            string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{ApplicationName}";
            DirectoryUtilities.CreateDirectory(basePath);
            return basePath;
        }

        public static string GetConfigurationDir()
        {
            string dir = $"{GetPlatformBaseDirectory()}\\Configurations";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetConfigurationDir(SocialNetworks network)
        {
            string dir = $"{GetPlatformBaseDirectory()}\\Configurations\\{network}";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }


        public const string CreateCampaign = "_Create Campaign";

        public const string UpdateCampaign = "_Update Campaign";

        public const string NoAccountSelected = "No Account Selected";


        public static string GetIndexAccountDir()
        {
            string dir = GetPlatformBaseDirectory() + @"\Index\AC";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetIndexAccountFile()
            => GetIndexAccountDir() + @"\AccountDetails.bin";


        public static string GetIndexCampaignDir()
        {
            string dir = GetPlatformBaseDirectory() + @"\Index\CP";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetCachePathDirectory()
        {
            var dir = GetPlatformBaseDirectory() + @"\Cache";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetIndexCampaignFile() => GetIndexCampaignDir() + @"\CampaignDetails.bin";


        public static string GetTemplatesFile() => GetConfigurationDir() + "\\Template.bin";


        public const string UnGrouped = "Ungrouped";

        public const string NotYetUpdate = "Not yet update";

        public const string NotChecked = "Not Checked";

        public static string DateasFileName { get; set; } = DateTime.Now.ToString("ddMMyyyyHmmss");


        public static string DateAsName { get; set; } = DateTime.Now.ToString("ddMMyyyyHmmssfffff");
        public static string GoogleLink { get; set; } = "https://www.google.com";

        public static string GetOtherDir()
        {
            string dir = $"{GetPlatformBaseDirectory()}\\Other";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetOtherProxyFile() => GetOtherDir() + @"\Proxy.bin";
        public static string GetOtherPostsFile() => GetOtherDir() + @"\Posts.bin";
        public static string GetOtherConfigFile() => GetOtherDir() + @"\Config.bin";

        public static string GetPublisherFile() => GetOtherDir() + @"\PublisherAccountDetails.bin";

        public static string GetChatDir()
        {
            string dir = $"{GetPlatformBaseDirectory()}\\Chat";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        internal static string GetLiveChatFile() => GetChatDir() + @"\LiveChat.bin";

        public static string GetDownloadedMediaFolderPath =>
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static string GetOtherEmailNotificationFile() => GetOtherDir() + @"\EmailNotification.bin";
        public static string GetOtherEmbeddedBrowserSettingsFile() => GetOtherDir() + @"\EmbeddedBrowserSettings.bin";
        public static string GetOtherSoftwareSettingsFile() => GetOtherDir() + @"\SoftwareSettings.bin";
        public static string GetOtherFacebookSettingsFile() => GetOtherDir() + @"\Facebook.bin";
        public static string GetOtherInstagramSettingsFile() => GetOtherDir() + @"\Instagram.bin";
        public static string GetConfigurationKey() => $"{GetConfigurationDir()}\\{ApplicationName}Key.bin";


        public static string GetAccountDb = "AccountDb";
        public static string GetCampaignDb = "CampaignDb";

    }
}
