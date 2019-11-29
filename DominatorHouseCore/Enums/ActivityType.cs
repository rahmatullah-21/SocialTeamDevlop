using System.ComponentModel;

namespace DominatorHouseCore.Enums
{
    public enum ActivityType
    {
        [Description("Instagram")]
        Follow = 1,

        [Description("Instagram")]
        Unfollow = 2,

        [Description("Instagram")]
        Like = 3,

        [Description("Instagram")]
        Unlike = 4,

        [Description("Instagram")]
        Comment = 5,

        //,Instagram,Reddit
       // [Description("Twitter")]
        DeleteComment = 6,
    

        //[Description("Instagram")]
        Post = 7,


        [Description("Instagram")]
        DeletePost = 9,
        [Description("Instagram")]
        UserScraper = 11,

        [Description("Instagram")]
        Reposter = 13,

        [Description("Instagram")]
        CommentScraper = 29,

        [Description("Instagram")]
        PostScraper = 30,

        [Description("Instagram")]
        BlockFollower = 56,
        [Description("Instagram")]
        LikeComment = 57,

        [Description("Instagram")]
        HashtagsScraper = 58,

        [Description("Instagram")]
        FollowBack = 60,

        [Description("Instagram")]
        BroadcastMessages = 70,

        [Description("Instagram")]
        SendMessageToFollower = 71,

        [Description("Instagram")]
        AutoReplyToNewMessage = 72,

        [Description("Instagram")]
        ReplyToComment = 78,

        [Description("Instagram")]
        StoryViewer = 118,


    }
}
