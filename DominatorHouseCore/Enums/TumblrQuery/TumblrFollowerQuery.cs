using System.ComponentModel;

namespace DominatorHouseCore.Enums.TumblrQuery
{
    public enum TumblrFollowerQuery
    {
        [Description("TumlangKeyword")]
        Keyword,
        [Description("TumlangUserFollower")]
        UserFollower,
        [Description("TumlangUserFollowing")]
        UserFollowing,
        [Description("TumlangHashTag")]
        HashtagUsers
    }
   
}