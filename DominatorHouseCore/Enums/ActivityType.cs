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
        DeletePost = 8,
        [Description("Instagram")]
        UserScraper = 9,

        [Description("Instagram")]
        Reposter = 10,

        [Description("Instagram")]
        CommentScraper = 11,

        [Description("Instagram")]
        PostScraper = 12,

        [Description("Instagram")]
        BlockFollower = 13,
        [Description("Instagram")]
        LikeComment = 14,

        [Description("Instagram")]
        HashtagsScraper = 15,

        [Description("Instagram")]
        FollowBack = 16,

        [Description("Instagram")]
        BroadcastMessages = 17,

        [Description("Instagram")]
        SendMessageToFollower = 18,

        [Description("Instagram")]
        AutoReplyToNewMessage = 19,

        [Description("Instagram")]
        ReplyToComment = 20,

        [Description("Instagram")]
        StoryViewer = 21,

        [Description("Instagram")]
        DownloadScraper = 22,
    }
}
