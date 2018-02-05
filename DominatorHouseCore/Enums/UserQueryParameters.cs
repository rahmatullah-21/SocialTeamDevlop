using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums
{
    public enum UserQueryParameters
    {
        [Description("langHashTagPost")]
        HashtagPost = 1,
        [Description("langHashtagusers")]
        HashtagUsers = 2,
        [Description("langKeywords")]
        Keywords = 3,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers = 4,
        [Description("langSomeonesFollowings")]
        SomeonesFollowings = 5,
        [Description("langFollowersOfSomeonesFollowings")]
        FollowersOfFollowings = 6,
        [Description("langFollowersOfSomeonesFollowers")]
        FollowersOfFollowers = 7,
        [Description("langLocationUsers")]
        LocationUsers = 8,
        [Description("langLocationPosts")]
        LocationPosts = 9,
        [Description("langCustomUser")]
        CustomUsers = 10,
        [Description("SuggestedUsers")]
        SuggestedUsers = 11,
        [Description("langCustomPhotos")]
        CustomPhotos = 12,
        [Description("langUsersWhoLiked")]
        UsersWhoLikedPost = 13,
        [Description("langUsersWhoCommented")]
        UsersWhoCommentedOnPost = 14
    }
}
