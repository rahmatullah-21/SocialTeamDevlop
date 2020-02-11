using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum GroupJoinerParameter
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,

        [Description("LangKeyGraphSearchUrl")]
        GraphSearchUrl = 2,

        [Description("LangKeyCustomGroupUrl")]
        CustomGroupUrl = 3
    }

    public enum GroupScraperParameter
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,

        [Description("LangKeyGraphSearchUrl")]
        GraphSearchUrl = 2,

        /*
                [Description("LangKeyCustomGroupUrl")]
                CustomGroupUrl = 3
        */


    }

    public enum FanpageLikerQueryParameters
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyGraphSearchUrl")]
        GraphSearchUrl = 2,
        [Description("LangKeyCustomPageUrlS")]
        CustomPageList = 3,
        [Description("LangKeyPagesLikedByFriends")]
        PagesLikedByFriends = 4
    }

    public enum FdFanpageLikerQuery
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyGraphSearchUrl")]
        GraphSearchUrl = 2,
        [Description("LangKeyCustomPageList")]
        CustomPageList = 3
    }

    public enum FdGroupJoinerQuery
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyCustomGroupUrlS")]
        CustomGroupUrl = 2
    }

    public enum FdProfileQueryParameters
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyLocation")]
        Location = 2,
        [Description("LangKeyOwnFriends")]
        OwnFriends = 3,        
        [Description("LangKeyCustomProfileUrl")]
        CustomProfileUrl = 4,
        [Description("LangKeyUserFollower")]
        UserFollowers = 5,        
        [Description("LangKeyPostLikers")]
        PostLikers = 6,
        [Description("LangKeyPostSharer")]
        PostSharer = 7,
        [Description("LangKeyPostCommentors")]
        PostCommentor = 8,        
        [Description("LangKeyFanpageLikers")]
        FanpageLikers = 9,
        [Description("LangKeyPagePostLikers")]
        PagePostLikers = 10,
        [Description("LangKeyPagePostCommenters")]
        PagePostPostCommenters = 11,
        [Description("LangKeyGroupMembers")]
        GroupMembers = 12,
        [Description("LangKeyGroupPostLikers")]
        GroupPostLikers = 13,
        [Description("LangKeyGroupPostCommenters")]
        GroupPostCommenters = 14,
        [Description("LangKeyEventUrl")]
        EventUrl = 15,
        [Description("LangKeyFriendOfFriend")]
        FriendofFriend = 16
        //[Description("LangKeyGraphSearchUrl")]
        //GraphSearchUrl = 9,      
        //[Description("LangKeySuggestedFriends")]
        //SuggestedFriends = 13,                      
        //[Description("LangKeyFriendsBasicDetails")]
        //FriendsBasicDetails = 17,
        //[Description("LangKeyGroupMemberBasicDetails")]
        //GroupMemberBasicDetails = 18,        
        //[Description("LangKeyPeopleConnectedInMessenger")]
        //ConnectedPeopleInMessenger = 21,
    }

    public enum FdProfileScraperQuery
    {
        [Description("LangKeyEvents")]
        Events = 1,
        [Description("LangKeyFanpageLikers")]
        FanpageLikers = 2,
        [Description("LangKeyFriendOfFriend")]
        FriendofFriend = 3,
        [Description("LangKeyGroupMembers")]
        GroupMembers = 4,
        [Description("LangKeyPostLikers")]
        PostLikers = 5,
        [Description("LangKeyPostSharer")]
        PostSharer = 6,
        [Description("LangKeyPostCommentors")]
        PostCommentor = 7,
        [Description("LangKeyGraphSearchUrl")]
        GraphSearchUrl = 8,
        [Description("LangKeyCustomUsersList")]
        CustomUserList = 9
    }

    public enum WebCommentLikerParameter
    {
        [Description("LangKeywebpagecomment")]
        Webpagecomment = 1,
        [Description("LangKeywebpagereplycomments")]
        Webpagereplycomments = 2
    }

    public enum CommentScraperParameter
    {
        [Description("LangKeyPostUrl")]
        PostUrl = 1,
        [Description("LangKeyPagePostComments")]
        PagePostComments = 2,
        [Description("LangKeyGroupPostComments")]
        GroupPostComments = 3,
        [Description("LangKeyNewsFeedPosts")]
        NewsFeedPosts = 4

    }

    public enum CommentLikerParameter
    {
        [Description("LangKeyPostUrl")]
        PostUrl = 1,
        [Description("LangKeyPagePostComments")]
        PagePostComments = 2,
        [Description("LangKeyGroupPostComments")]
        GroupPostComments = 3,
        [Description("LangKeyNewsFeedPosts")]
        NewsFeedPosts = 4,
    }

    public enum MarketPlaceQueryParameter
    {
        [Description("LangKeyKeywords")]
        Keywords = 1
    }

    public enum FdUserQueryParameters
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyLocation")]
        Location = 2,
        [Description("LangKeyCustomProfileUrl")]
        CustomProfileUrl = 3,
        [Description("LangKeyUserFollower")]
        UserFollowers = 4,
        [Description("LangKeyPostLikers")]
        PostLikers = 5,
        [Description("LangKeyPostSharer")]
        PostSharer = 6,
        [Description("LangKeyPostCommentors")]
        PostCommentor = 7,
        [Description("LangKeyFanpageLikers")]
        FanpageLikers = 8,
        [Description("LangKeyPagePostLikers")]
        PagePostLikers = 9,
        [Description("LangKeyPagePostCommenters")]
        PagePostCommenters = 10,
        [Description("LangKeyGroupMembers")]
        GroupMembers = 11,
        [Description("LangKeyGroupPostLikers")]
        GroupPostLikers = 12,
        [Description("LangKeyGroupPostCommenters")]
        GroupPostCommenters = 13,
        [Description("LangKeyEventUrl")]
        EventUrl = 14,
        [Description("LangKeyFriendOfFriend")]
        FriendofFriend = 15
        //[Description("LangKeyGraphSearchUrl")]
        //GraphSearchUrl = 9,

        //[Description("LangKeySuggestedFriends")]
        //SuggestedFriends = 15,


    }
}
