using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums.TumblrQuery
{
    public enum TumblrQuery
    {
        [Description("TumlangKeyword")]
        Keyword,
        [Description("TumlangUserFollowing")]
        UserFollowing,
        [Description("TumlangUserFollower")]
        UserFollower,
        [Description("TumlangHashTag")]
        HashtagUsers,
        [Description("TumlangUserCommentedOnPost")]
        UserCommentedOnPost,
        [Description("TumlangUserLikedThePost")]
        UserLikedThePost,
        [Description("TumlangUserReblogedThePost")]
        UserReblogedThePost,
        [Description("TumlangUserLikedCommentedReblogedThePost")]
        UserLikedCommentedReblogedThePost
    }
}
