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
            var path = string.Empty;

            switch (SocialNetwork)
            {
                case SocialNetworks.Instagram:
                    path = $"{ConstantVariable.GetConfigurationPath(SocialNetworks.Instagram)}";//GetConfigurationPath(SocialNetworks.Instagram)
                    break;
                case SocialNetworks.Twitter:
                    path = $"{ConstantVariable.GetConfigurationPath(SocialNetworks.Twitter)}";//GetConfigurationPath(SocialNetworks.Instagram)
                    break;
            }

            return path;
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