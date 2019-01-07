using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models.SocioPublisher;
using System;
using System.Collections.Generic;
using System.IO;


namespace DominatorHouseCore.Utility
{
    public interface IConstantVariable
    {
        string UseragentCommonFormat { get; }

        string UseragentLocale { get; }

        // string IgVersion => "40.33.0";
        string IgVersion { get; }

        string ApiUrl { get; }

        string InstagramBaseUrl { get; }

        string XFbHttpEngine { get; }

        int WebrequestTimeout { get; }

        bool UseSystemProxy { get; }

        string ApplicationName { get; }

        string BitlyApiKey { get; set; }

        string BitlyLogin { get; set; }

        string Revision { get; set; }


        string GetPlatformBaseDirectory();


        string GetPlatformTodayBackupDirectory();



        string GetPlatformLogDirectory();


        string GetConfigurationDir();

        string GetConfigurationDir(SocialNetworks network);


        string CreateCampaign { get; }

        string UpdateCampaign { get; }

        string NoAccountSelected { get; }


        string GetIndexAccountDir();

        string GetIndexAccountFile();


        string GetIndexCampaignDir();

        string GetCachePathDirectory();

        string GetIndexCampaignFile();


        string GetTemplatesFile();


        string UnGrouped { get; }

        string NotYetUpdate { get; }

        string NotChecked { get; }

        string DateasFileName { get; }

        string GetDate();

        string GetDateTime();

        string GetHourDateTime();

        string GoogleLink { get; }

        string GetOtherDir();

        string GetOtherProxyFile();
        string GetOtherPostsFile();

        string GetOtherConfigFile();

        string GetPublisherFile();

        string GetPublisherDestinationsFile();


        string GetPublisherCreatePostlistFolder();

        string GetPublisherCreateDestinationsFolder();

        string GetPublisherPostlistSettingsFile();

        string GetChatDir();

        string GetLiveChatFile();

        string GetLiveChatFile(SocialNetworks network);

        string GetDownloadedMediaFolderPath { get; }

        string GetNotFoundImage();

        string GetSocinatorIcon();

        string GetOtherEmailNotificationFile();

        string GetOtherEmbeddedBrowserSettingsFile();

        string GetOtherSoftwareSettingsFile();

        string GetOtherFacebookSettingsFile();
        string GetOtherInstagramSettingsFile();
        string GetConfigurationKey();
        string GetURLShortnerServicesFile();

        string GetCaptchaServicesFile();

        string GetImageCaptchaServicesFile();

        string SaveAction { get; }

        string UpdateAction { get; }
        string GetFavoriteTimeFile();
        string GetOtherQuoraSettingsFile();
        string GetOtherLinkedInSettingsFile();

        #region Publisher

        string FineStatusSync { get; }

        string NeedUpdateStatusSync { get; }

        string NotAvailableAccountSync { get; }

        string DraftPostList { get; }

        string PendingPostList { get; }

        string PublishedPostList { get; }
        string GetPublisherOtherConfigDir();

        string GetPublisherOtherConfigFile(SocialNetworks networks);

        string GetPublisherCampaignFile();

        string GetPublisherPostFetchFile { get; }

        string GetMacroDetails { get; }

        string GetPublishedSuccessDetails { get; }

        string GetDeletePublisherPostModelGetDeletePublisherPostModel { get; }

        string Yes { get; }

        string No { get; }

        string NoError { get; }

        DateTime GetCurrentDateTime();

        string DeletedDateText();

        string NotPublished { get; }

        #endregion


        string GetAccountDb { get; }

        string GetCampaignDb { get; }

        IEnumerable<SocinatorIntellisenseModel> FdMacros { get; }

        string Separator { get; }

        string VideoToImageConvertFileName { get; }

        List<string> SupportedVideoFormat { get; }

        string VideoToImageConvertPngFileName { get; }
        string ProcessingInput { get; }

        string ProcessingDebugType { get; }

        string FindExemptions { get; }

        string ExemptionInnerException { get; }

        string LogExemptions { get; }

        string DebugLogExemptions { get; }

        string LogDebugExemption { get; }

        string MarketingSoftware { get; }

        string ContactSupportLink { get; }

        bool IsToasterNotificationNeed { get; }


        string PageOrBoard { get; }

        string Group { get; }

        string OwnWall { get; }
        string UpdatedVersionIP { get; }

        string UpdateVersionFilePath { get; }
        string UpdateVersionLink { get; }

        string GetFacebookDetailsConfigFile();

        string GetDeletePublisherPostModel { get; }
    }

    public class ConstantVariable : IConstantVariable
    {
        public string UseragentCommonFormat => "Instagram {0} Android ({1}/{2}; {3}; {4}; {5}; {6}; {7}; {8}; {9})";

        public string UseragentLocale => "en_US";

        // public string IgVersion => "40.33.0";
        public string IgVersion => "64.0.0.14.96";

        public string ApiUrl => $"{(object)InstagramBaseUrl}api/v1/";

        public string InstagramBaseUrl => "https://i.instagram.com/";

        public string XFbHttpEngine => "Liger";

        public int WebrequestTimeout => 20000;

        public bool UseSystemProxy => true;

        public string ApplicationName => "Socinator";

        public string BitlyApiKey { get; set; } = string.Empty;

        public string BitlyLogin { get; set; } = string.Empty;

        public string Revision { get; set; } = string.Empty;


        public string GetPlatformBaseDirectory()
        {
            string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{ApplicationName}";

            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }

        public string GetPlatformTodayBackupDirectory()
        {
            string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{ApplicationName}Backup";

            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }


        public string GetPlatformLogDirectory()
        {
            string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{ApplicationName}\\logs";

            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }


        public string GetConfigurationDir()
        {
            string dir = $"{GetPlatformBaseDirectory()}\\Configurations";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public string GetConfigurationDir(SocialNetworks network)
        {
            string dir = $"{GetPlatformBaseDirectory()}\\Configurations\\{network}";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }


        public string CreateCampaign => "_Create Campaign";

        public string UpdateCampaign => "_Update Campaign";

        public string NoAccountSelected => "No Account Selected";


        public string GetIndexAccountDir()
        {
            string dir = GetPlatformBaseDirectory() + @"\Index\AC";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public string GetIndexAccountFile()
            => GetIndexAccountDir() + @"\AccountDetails.bin";


        public string GetIndexCampaignDir()
        {
            string dir = GetPlatformBaseDirectory() + @"\Index\CP";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public string GetCachePathDirectory()
        {
            var dir = GetPlatformBaseDirectory() + @"\Cache";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public string GetIndexCampaignFile() => GetIndexCampaignDir() + @"\CampaignDetails.bin";


        public string GetTemplatesFile() => GetConfigurationDir() + "\\Template.bin";


        public string UnGrouped => "Ungrouped";

        public string NotYetUpdate => "Not yet update";

        public string NotChecked => "Not Checked";

        public string DateasFileName => DateTime.Now.ToString("ddMMyyyyHmmss");

        public string GetDate() => DateTime.Now.ToString("ddMMyyyy");

        public string GetDateTime() => DateTime.Now.ToString("ddMMyyyyHmmss");

        public string GetHourDateTime() => DateTime.Now.ToString("Hmmss.ff");

        public string GoogleLink => "https://www.google.com";

        public string GetOtherDir()
        {
            string dir = $"{GetPlatformBaseDirectory()}\\Other";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public string GetOtherProxyFile() => GetOtherDir() + @"\Proxy.bin";
        public string GetOtherPostsFile() => GetOtherDir() + @"\Posts.bin";
        public string GetOtherConfigFile() => GetOtherDir() + @"\Config.bin";

        public string GetPublisherFile() => GetOtherDir() + @"\PublisherAccountDetails.bin";

        public string GetPublisherDestinationsFile() => GetOtherDir() + @"\PublisherManageDestinations.bin";


        public string GetPublisherCreatePostlistFolder() => GetOtherDir() + @"\PostsList\";

        public string GetPublisherCreateDestinationsFolder() => GetOtherDir() + @"\DestinationList\";

        public string GetPublisherPostlistSettingsFile() => GetOtherDir() + @"\PostlistSettings.bin";

        public string GetChatDir()
        {
            string dir = $"{GetPlatformBaseDirectory()}\\LiveChat";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        internal string GetLiveChatFile() => GetChatDir() + @"\LiveChat.bin";
        public string GetLiveChatFile(SocialNetworks network) => GetChatDir() + $"\\{network}.bin";
        public string GetDownloadedMediaFolderPath =>
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string GetNotFoundImage() => GetOtherDir() + @"\NotFoundImage.png";
        public string GetSocinatorIcon() => GetOtherDir() + @"SocinatorIcon.png";
        public string GetOtherEmailNotificationFile() => GetOtherDir() + @"\EmailNotification.bin";
        public string GetOtherEmbeddedBrowserSettingsFile() => GetOtherDir() + @"\EmbeddedBrowserSettings.bin";
        public string GetOtherSoftwareSettingsFile() => GetOtherDir() + @"\SoftwareSettings.bin";
        public string GetOtherFacebookSettingsFile() => GetOtherDir() + @"\Facebook.bin";
        public string GetOtherInstagramSettingsFile() => GetOtherDir() + @"\Instagram.bin";
        public string GetConfigurationKey() => $"{GetConfigurationDir()}\\{ApplicationName}Key.bin";
        public string GetURLShortnerServicesFile() => GetOtherDir() + @"\URLShortnerServices.bin";
        public string GetCaptchaServicesFile() => GetOtherDir() + @"\CaptchaServices.bin";
        public string GetImageCaptchaServicesFile() => GetOtherDir() + @"\ImageCaptchaServices.bin";
        public string SaveAction => "Save";

        public string UpdateAction => "Update";
        public string GetFavoriteTimeFile() => GetOtherDir() + @"\FavoriteTime.bin";
        public string GetOtherQuoraSettingsFile() => GetOtherDir() + @"\Quora.bin";
        public string GetOtherLinkedInSettingsFile() => GetOtherDir() + @"\LinkedIn.bin";
        #region Publisher

        public string FineStatusSync => "Already up to date";

        public string NeedUpdateStatusSync => "Click to Sync";

        public string NotAvailableAccountSync => "Account not available";

        public string DraftPostList => "Draft";

        public string PendingPostList => "Pending";

        public string PublishedPostList => "Published";
        public string GetPublisherOtherConfigDir()
        {
            string dir = $"{GetOtherDir() }\\PublisherOtherConfig";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public string GetPublisherOtherConfigFile(SocialNetworks networks) => GetPublisherOtherConfigDir() + $"\\{networks}.bin";

        public string GetPublisherCampaignFile() => GetOtherDir() + "\\PublisherCampaign.bin";

        public string GetPublisherPostFetchFile => GetOtherDir() + "\\PublisherPostFetcherDetails.bin";

        public string GetMacroDetails => GetOtherDir() + "\\SocinatorMacros.bin";

        public string GetPublishedSuccessDetails => GetOtherDir() + "\\PublishedSuccessDetails.bin";

        public string GetDeletePublisherPostModel => GetOtherDir() + "\\PublisherDeletionPosts.bin";

        public string Yes => "Yes";

        public string No => "No";

        public string NoError => "No Error!";

        public DateTime GetCurrentDateTime() => DateTime.Now;

        public string DeletedDateText() => $"Post has been deleted on {GetCurrentDateTime()}!";

        public string NotPublished => "Not Published Yet";

        #endregion


        public string GetAccountDb => "AccountDb";

        public string GetCampaignDb => "CampaignDb";

        public IEnumerable<SocinatorIntellisenseModel> FdMacros => new List<SocinatorIntellisenseModel>
        {
            new SocinatorIntellisenseModel() {Key="{AllFriends}",Value =  "{AllFriends}"},
            new SocinatorIntellisenseModel() {Key="{Random:1-5}",Value =  "{Random:1-5}"},
            new SocinatorIntellisenseModel() {Key="{Random:5-10}",Value =  "{Random:5-10}"},
            new SocinatorIntellisenseModel() {Key="{Random:15-20}",Value =  "{Random:15-20}"},
            new SocinatorIntellisenseModel() {Key="{Random:10-15}",Value =  "{Random:10-15}"},
            new SocinatorIntellisenseModel() {Key="{Random:20+}",Value =  "{Random:20+}"},

        };

        public string Separator => "<:>";

        public string VideoToImageConvertFileName => "_SOCINATORIMAGE.jpg";

        public List<string> SupportedVideoFormat => new List<string> { "3g2", "3gp", "3gpp", "asf", "avi", "dat", "divx", "dv", "f4v", "flv", "m2ts", "m4v", "mkv", "mod", "mov", "mp4", "mpe", "mpeg", "mpeg4", "mpg", "mts", "nsv", "ogm", "ogv", "qt", "tod", "ts", "vob", "wmv" };

        public string VideoToImageConvertPngFileName => "_SOCINATORIMAGE.png";
        public string ProcessingInput =>
            "https://socinator.com/amember/softsale/api/check-activation?key={0}&request[hardware-id]={1}";

        public string ProcessingDebugType =>
            "https://dominatorhouse.com/amember/softsale/api/check-activation?key={0}&request[hardware-id]={1}";

        public string FindExemptions =>
            "https://socinator.com/amember/softsale/api/check-license?key={0}";

        public string ExemptionInnerException
            => "https://socinator.com/amember/api/invoices/{0}?_key={1}";

        public string LogExemptions
            => "https://socinator.com/amember/softsale/api/activate?key={0}&request[hardware-id]={1}";

        public string DebugLogExemptions
            => "https://dominatorhouse.com/amember/softsale/api/activate?key={0}&request[hardware-id]={1}";

        public string LogDebugExemption
            => "https://socinator.com/amember/softsale/api/deactivate?key={0}&request[hardware-id]={1}";


        public string MarketingSoftware => "Marketing Software";

        public string ContactSupportLink => "https://socinator.com/contact-us/";

        public bool IsToasterNotificationNeed => true;


        public string PageOrBoard => "PageOrBoard";

        public string Group => "Group";

        public string OwnWall => "OwnWall";
        public string UpdatedVersionIP => "169.50.161.212";

        public string UpdateVersionFilePath => "fd/setup/FDSetup.txt";
        public string UpdateVersionLink =>
              "http://{0}/{1}";

        public string GetDeletePublisherPostModelGetDeletePublisherPostModel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string GetFacebookDetailsConfigFile() => GetOtherDir() + @"\FacebokDetails\FacebookEntity.bin";

        string IConstantVariable.GetLiveChatFile()
        {
            throw new NotImplementedException();
        }
    }

    public static class FileDirPath
    {
        public static string GetChatDir(SocialNetworks network)
        {
            IConstantVariable constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();
            string dir = $"{constantVariable.GetPlatformBaseDirectory()}\\LiveChat\\{network}";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetChatDetailFile(SocialNetworks network) => GetChatDir(network) + "\\Chat.bin";
        public static string GetFriendDetailFile(SocialNetworks network) => GetChatDir(network) + "\\Friend.bin";
    }
}
