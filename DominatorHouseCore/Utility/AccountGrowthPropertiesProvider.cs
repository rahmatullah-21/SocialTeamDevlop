using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel;
using System.Collections.Generic;

namespace DominatorHouseCore.Utility
{
    public interface IAccountGrowthPropertiesProvider
    {
        IList<GrowthProperty> this[SocialNetworks network] { get; }
    }

    public class AccountGrowthPropertiesProvider : IAccountGrowthPropertiesProvider
    {
        private readonly IDictionary<SocialNetworks, List<GrowthProperty>> _growthProperties;

        public AccountGrowthPropertiesProvider()
        {
            _growthProperties = new Dictionary<SocialNetworks, List<GrowthProperty>>
            {
                {
                    SocialNetworks.Instagram, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "LangKeyFollowers".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty {PropertyName = "LangKeyFollowings".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty {PropertyName = "LangKeyUploads".FromResourceDictionary(), PropertyValue = 0},
                    }
                },               
                {
                    SocialNetworks.Social, new List<GrowthProperty>()
                }
            };
        }

        public IList<GrowthProperty> this[SocialNetworks network]
        {
            get { return _growthProperties[network]; }
        }
    }
}
