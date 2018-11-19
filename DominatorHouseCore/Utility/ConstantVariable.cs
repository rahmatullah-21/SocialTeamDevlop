using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.SocioPublisher;
using System;
using System.Collections.Generic;
using System.IO;


namespace DominatorHouseCore.Utility
{
    public static class ConstantVariable
    {
        public static string UseragentCommonFormat { get; } = "Instagram {0} Android ({1}/{2}; {3}; {4}; {5}; {6}; {7}; {8}; {9})";

        public static string UseragentLocale { get; } = "en_US";

        // public static string IgVersion { get; } = "40.33.0";
        public static string IgVersion { get; } = "64.0.0.14.96";

        public static string ApiUrl => $"{(object)ConstantVariable.InstagramBaseUrl}api/v1/";

        public static string InstagramBaseUrl { get; } = "https://i.instagram.com/";

        public static string XFbHttpEngine { get; } = "Liger";

        public static int WebrequestTimeout { get; } = 20000;

        public static bool UseSystemProxy { get; } = true;

        public static string ApplicationName { get; } = "Socinator";

        public static string BitlyApiKey { get; set; } = string.Empty;

        public static string BitlyLogin { get; set; } = string.Empty;
        public static string Revision { get; set; }


        public static string GetPlatformBaseDirectory()
        {
            string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{ApplicationName}";

            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }

        public static string GetPlatformTodayBackupDirectory()
        {
            string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{ApplicationName}Backup";

            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }


        public static string GetPlatformLogDirectory()
        {
            string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{ApplicationName}\\logs";

            if (!Directory.Exists(basePath))
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
            string dir = $"{GetPlatformBaseDirectory()}\\LiveChat";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        internal static string GetLiveChatFile() => GetChatDir() + @"\LiveChat.bin";
        public static string GetLiveChatFile(SocialNetworks network) => GetChatDir() + $"\\{network}.bin";
        public static string GetDownloadedMediaFolderPath =>
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static string GetNotFoundImage() => GetOtherDir() + @"\NotFoundImage.png";
        public static string GetSocinatorIcon() => GetOtherDir() + @"SocinatorIcon.png";
        public static string GetOtherEmailNotificationFile() => GetOtherDir() + @"\EmailNotification.bin";
        public static string GetOtherEmbeddedBrowserSettingsFile() => GetOtherDir() + @"\EmbeddedBrowserSettings.bin";
        public static string GetOtherSoftwareSettingsFile() => GetOtherDir() + @"\SoftwareSettings.bin";
        public static string GetOtherFacebookSettingsFile() => GetOtherDir() + @"\Facebook.bin";
        public static string GetOtherInstagramSettingsFile() => GetOtherDir() + @"\Instagram.bin";
        public static string GetConfigurationKey() => $"{GetConfigurationDir()}\\{ApplicationName}Key.bin";
        public static string GetURLShortnerServicesFile() => GetOtherDir() + @"\URLShortnerServices.bin";
        public static string GetCaptchaServicesFile() => GetOtherDir() + @"\CaptchaServices.bin";
        public static string SaveAction { get; set; } = "Save";

        public static string UpdateAction { get; set; } = "Update";
        public static string GetFavoriteTimeFile() => GetOtherDir() + @"\FavoriteTime.bin";
        public static string GetOtherQuoraSettingsFile() => GetOtherDir() + @"\Quora.bin";
        public static string GetOtherLinkedInSettingsFile() => GetOtherDir() + @"\LinkedIn.bin";
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

        public static string GetDeletePublisherPostModel => GetOtherDir() + "\\PublisherDeletionPosts.bin";

        public static string Yes { get; set; } = "Yes";

        public static string No { get; set; } = "No";

        public static string NoError { get; set; } = "No Error!";

        public static DateTime GetCurrentDateTime() => DateTime.Now;

        public static string DeletedDateText() => $"Post has been deleted on {GetCurrentDateTime()}!";

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


        public static string ProcessingInput { get; set; } =
            "https://socinator.com/amember/softsale/api/check-activation?key={0}&request[hardware-id]={1}";

        public static string ProcessingDebugType { get; set; } =
            "https://dominatorhouse.com/amember/softsale/api/check-activation?key={0}&request[hardware-id]={1}";

        public static string FindExemptions { get; set; } =
            "https://socinator.com/amember/softsale/api/check-license?key={0}";

        public static string ExemptionInnerException { get; set; }
            = "https://socinator.com/amember/api/invoices/{0}?_key={1}";

        public static string LogExemptions { get; set; }
            = "https://socinator.com/amember/softsale/api/activate?key={0}&request[hardware-id]={1}";

        public static string DebugLogExemptions { get; set; }
            = "https://dominatorhouse.com/amember/softsale/api/activate?key={0}&request[hardware-id]={1}";

        public static string LogDebugExemption { get; set; }
            = "https://socinator.com/amember/softsale/api/deactivate?key={0}&request[hardware-id]={1}";


        public static string MarketingSoftware { get; set; } = "Marketing Software";
        public static string ContactSupportLink { get; set; } = "http://help.socinator.com/support/home";

        public static bool IsToasterNotificationNeed { get; set; } = true;


        public static string PageOrBoard { get; set; } = "PageOrBoard";

        public static string Group { get; set; } = "Group";

        public static string OwnWall { get; set; } = "OwnWall";
        public static string UpdatedVersionIP { get; set; } = "169.50.161.212";

        public static string UpdateVersionFilePath { get; set; } = "fd/setup/FDSetup.txt";
        public static string UpdateVersionLink { get; set; } =
              "http://{0}/{1}";

        public static string GetFacebookDetailsConfigFile() => GetOtherDir() + @"\FacebokDetails\FacebookEntity.bin";
    }

    public static class FileDirPath
    {
        public static string GetChatDir(SocialNetworks network)
        {
            string dir = $"{ConstantVariable.GetPlatformBaseDirectory()}\\LiveChat\\{network}";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetChatDetailFile(SocialNetworks network) => GetChatDir(network) + $"\\Chat.bin";
        public static string GetFriendDetailFile(SocialNetworks network) => GetChatDir(network) + $"\\Friend.bin";
    }
}
