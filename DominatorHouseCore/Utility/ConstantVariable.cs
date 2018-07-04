using System;

using System.Collections.Generic;
using System.Windows.Documents;
using DominatorHouseCore.Enums;
using System.Reflection;
using DominatorHouseCore.Diagnostics;
using System.IO;
using DominatorHouseCore.Models.SocioPublisher;

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

        public static string BitlyApiKey { get; set; } = string.Empty;

        public static string BitlyLogin { get; set; } = string.Empty;

        public static string GetPlatformBaseDirectory()
        {
            string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{ApplicationName}";

            if(!Directory.Exists(basePath))
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

        public static string GetDate() => DateTime.Now.ToString("ddMMyyyy");

        public static string GetDateTime() => DateTime.Now.ToString("ddMMyyyyHmmss");

        public static string GetHourDateTime() => DateTime.Now.ToString("Hmmss.ff");

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

        public static string GetPublisherDestinationsFile() => GetOtherDir() + @"\PublisherManageDestinations.bin";


        public static string GetPublisherCreatePostlistFolder() => GetOtherDir() + @"\PostsList\";

        public static string GetPublisherCreateDestinationsFolder() => GetOtherDir() + @"\DestinationList\";

        public static string GetPublisherPostlistSettingsFile() => GetOtherDir() + @"\PostlistSettings.bin";

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

        public static string SaveAction { get; set; } = "Save";

        public static string UpdateAction { get; set; } = "Update";

        #region Publisher

        public static string FineStatusSync = "Already up to date";

        public static string NeedUpdateStatusSync = "Click to Sync";

        public static string NotAvailableAccountSync = "Account not available";

        public static string DraftPostList { get; set; } = "Draft";

        public static string PendingPostList { get; set; } = "Pending";

        public static string PublishedPostList { get; set; } = "Published";
        public static string GetPublisherOtherConfigDir()
        {
            string dir = $"{GetOtherDir() }\\PublisherOtherConfig";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetPublisherOtherConfigFile(SocialNetworks networks) => GetPublisherOtherConfigDir() + $"\\{networks}.bin";

        public static string GetPublisherCampaignFile() => GetOtherDir() + "\\PublisherCampaign.bin";

        public static string GetPublisherPostFetchFile => GetOtherDir() + "\\PublisherPostFetcherDetails.bin";

        public static string GetMacroDetails => GetOtherDir() + "\\SocinatorMacros.bin";

        public static string GetPublishedSuccessDetails => GetOtherDir() + "\\PublishedSuccessDetails.bin";

        public static  string GetDeletePublisherPostModel => GetOtherDir() + "\\PublisherDeletionPosts.bin";

        public static string Yes { get; set; } = "Yes";

        public static string No { get; set; } = "No";

        public static string NoError { get; set; } = "No Error!";

        public static string NotPublished { get; set; } = "Not Published Yet";

        #endregion


        public static string GetAccountDb = "AccountDb";

        public static string GetCampaignDb = "CampaignDb";

        public static IEnumerable<SocinatorIntellisenseModel> FdMacros = new List<SocinatorIntellisenseModel>
        {
            new SocinatorIntellisenseModel() {Key="{AllFriends}",Value =  "{AllFriends}"},
            new SocinatorIntellisenseModel() {Key="{Random:1-5}",Value =  "{Random:1-5}"},
            new SocinatorIntellisenseModel() {Key="{Random:5-10}",Value =  "{Random:5-10}"},
            new SocinatorIntellisenseModel() {Key="{Random:15-20}",Value =  "{Random:15-20}"},
            new SocinatorIntellisenseModel() {Key="{Random:10-15}",Value =  "{Random:10-15}"},
            new SocinatorIntellisenseModel() {Key="{Random:20+}",Value =  "{Random:20+}"},

        };

        public static string Separator = "<:>";

        public static string VideoToImageConvertFileName { get; set; } = "_SOCINATORIMAGE.jpg";

        public static List<string> SupportedVideoFormat = new List<string> { "3g2", "3gp", "3gpp", "asf", "avi", "dat", "divx", "dv", "f4v", "flv", "m2ts", "m4v", "mkv", "mod", "mov", "mp4", "mpe", "mpeg", "mpeg4", "mpg", "mts", "nsv", "ogm", "ogv", "qt", "tod", "ts", "vob", "wmv" };
    }
}
