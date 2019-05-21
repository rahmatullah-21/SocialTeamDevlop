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
                    SocialNetworks.Twitter, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Followers", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Followings", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Tweets", PropertyValue = 0},
                    }
                },
                {
                    SocialNetworks.Facebook, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Friends", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "JoinedGroups", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "OwnPages", PropertyValue = 0},
                    }
                },
                {
                    SocialNetworks.Quora, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Followers", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Following", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Post", PropertyValue = 0},
                    }
                },
                {
                    SocialNetworks.Reddit, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Score", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Communities", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Post Karma", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Comment Karma", PropertyValue = 0},
                    }
                }
               // {
                    //SocialNetworks.Gplus, new List<GrowthProperty>
                    //{
                    //    new GrowthProperty {PropertyName = "Followers", PropertyValue = 0},
                    //    new GrowthProperty {PropertyName = "Followings", PropertyValue = 0},
                    //    new GrowthProperty {PropertyName = "Communities", PropertyValue = 0},
                    //}
               // }
                ,
                {
                    SocialNetworks.Youtube, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Channels Count", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Views Count", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Support Links Count", PropertyValue = 0},
                    }
                },
                {
                    SocialNetworks.Instagram, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Followers", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Followings", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Uploads", PropertyValue = 0},
                    }
                },
                {
                    SocialNetworks.LinkedIn, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Connections", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Groups", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Invitation", PropertyValue = 0},
                    }
                },
                {
                    SocialNetworks.Tumblr, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Followers", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Followings", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Posts", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Channels", PropertyValue = 0},
                    }
                },
                {
                    SocialNetworks.Pinterest, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "Followers", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Followings", PropertyValue = 0},
                        new GrowthProperty {PropertyName = "Post", PropertyValue = 0},
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
