using System.Reflection;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Utility
{

    public class SocialNetworkPathFactory
    {

        public SocialNetworks SocialNetwork { get; set; }

        public SocialNetworkPathFactory(SocialNetworks socialNetwork)
        {
            SocialNetwork = socialNetwork;
        }

        public string GetSocialNetworkConfigPath()
        {
            return  $"{ConstantVariable.GetConfigurationPath(SocialNetwork)}";
     
        }


        public string GetSocialNetworkCampaignPath()
        {
            var path = string.Empty;

           

            switch (SocialNetwork)
            {
                case SocialNetworks.Instagram:
                    path = $"{ConstantVariable.GetIndexCampaignPath(SocialNetworks.Instagram)}";
                    break;
            }

            return path;
        }
    }
}