using System;
using System.Windows.Documents;
using DominatorHouseCore.Enums;
using System.Reflection;

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

        public static string Manufacturer { get; } = "DOMINATORHOUSE";

        //  public static string GdPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{Manufacturer}\\GD";
        public static string GetDominatorBaseDirectory()
        {
            return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{Manufacturer}";
        }

        public static string GetDominatorPath(SocialNetworks SocialNetworkType)
        {
            return $"{GetDominatorBaseDirectory()}\\{SocialNetworkType}";
        }



     //   public static string GetConfigurationPath(SocialNetworks.Instagram) = $"{GdPath}\\Configurations";


        public static string GetConfigurationPath(SocialNetworks SocialNetworkType)
        {
            return $"{GetDominatorPath(SocialNetworkType)}\\Configurations";
        }

        public const string TemplateBinName = "Template.bin";

        public const string CreateCampaign = "Create Campaign";

        public const string UpdateCampaign = "Update Campaign";

        public const string NoAccountSelected = "No Account Selected";


        public static string GetIndexAccountPath()
        {
            return GetDominatorBaseDirectory()+ ConstantVariable.DominatorPath + "\\Index\\AC";
        }

        // Back compatibility
        public static string GetIndexAccountPath(SocialNetworks sc)
        {
            return GetIndexAccountPath();
        }

            // public static string GetIndexCampaignPath(SocialNetworks.Instagram) = GdPath + "\\Index\\CP";

        public static string GetIndexCampaignPath(SocialNetworks SocialNetworkType)
        {
            return GetDominatorPath(SocialNetworkType) + "\\Index\\CP";
        }



        public static string AccountDetails = "AccountDetails.bin";

        public static string CampaignDetails = "CampaignDetails.bin";

        public const string UnGrouped = "Ungrouped";

        public const string NotYetUpdate = "Not yet update";

        public const string NotChecked = "Not Checked";

        public static string DateasFileName { get; set; } = DateTime.Now.ToString("ddMMyyyyHmmss");


      //  public static string socialConfigurationPath = new SocialNetworkPathFactory(SocialNetworks.Instagram).GetSocialNetworkConfigPath();


        public static string socialConfigurationPath(SocialNetworks SocialNetworkType)
        {
           return new SocialNetworkPathFactory(SocialNetworkType).GetSocialNetworkConfigPath();
        }

     //   public static string socialNetworkPath = new SocialNetworkPathFactory(SocialNetworks.Instagram).GetSocialNetworkCampaignPath();

        public static string socialNetworkPath(SocialNetworks SocialNetworkType)
        {
            return new SocialNetworkPathFactory(SocialNetworkType).GetSocialNetworkCampaignPath();
        }

        public static string DominatorPath = Assembly.GetExecutingAssembly().FullName;

    }
}
