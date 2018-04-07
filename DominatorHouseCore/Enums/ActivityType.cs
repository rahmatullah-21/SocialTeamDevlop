using System.ComponentModel;

namespace DominatorHouseCore.Enums
{
    public enum ActivityType
    {
        [Description("Twitter,Instagram,Gplus")]
        Follow = 1,

        [Description("Twitter,Instagram,Gplus")]
        Unfollow = 2,

        [Description("Facebook,Twitter,Instagram,Gplus")]
        Like = 3,

        [Description("Twitter,Instagram,Gplus")]
        Unlike = 4,

        [Description("Facebook,Twitter,Instagram,Gplus")]
        Comment = 5,

        [Description("Twitter,Instagram")]
        DeleteComment = 6,

        [Description("Facebook,Twitter,Instagram")]
        Post = 7,

        // INFO : Ambiguous between Repost(8) and Reposter(13), so deleted, if any one used please with Reposter(13)
        // [Description("Twitter,Instagram")]
        // Repost = 8,

        [Description("Facebook,Twitter,Instagram")]
        DeletePost = 9,

        [Description("Facebook,Twitter,Instagram")]
        Message = 10,

        [Description("Facebook,Twitter,Instagram,Gplus,LinkedIn")]
        UserScraper = 11,

        [Description("Twitter,Instagram")]
        DownloadScraper = 12,

        [Description("Facebook,Twitter,Instagram")]
        Reposter = 13,

        [Description("Twitter")]
        Retweet = 14,

        [Description("Quora")]
        QuestionsScraper = 15,

        [Description("Quora")]
        AnswersScraper = 16,

        [Description("Quora")]
        VoteAnswers = 17,

        [Description("Quora")]
        DownvoteAnswers = 18,

        [Description("Quora")]
        ReportQuestions = 19,

        [Description("Quora")]
        ReportAnswers = 20,

        [Description("Quora")]
        ReportUsers = 21,

        [Description("Pinterest")]
        BoardScraper = 22,

        [Description("Pinterest")]
        PinScraper = 23,

        [Description("Facebook")]
        SendFriendRequest = 24,

        [Description("Facebook")]
        ManageFriendRequest = 25,

        [Description("Facebook")]
        Unfriend = 26,

        [Description("Facebook")]
        GroupScraper = 27,

        [Description("Facebook")]
        FanpageScraper = 28,

        [Description("Facebook")]
        CommentScraper = 29,

        [Description("Facebook,Gplus")]
        PostScraper = 30,

        [Description("Facebook,LinkedIn")]
        GroupJoiner = 31,

        [Description("Facebook")]
        GroupUnjoiner = 32,

        [Description("Facebook")]
        GroupInviter = 34,

        [Description("Facebook")]
        PageInviter = 35,

        [Description("Facebook")]
        EventInviter = 36,

        [Description("Facebook")]
        GroupCreator = 37,

        [Description("Facebook")]
        EventCreator = 38,

        [Description("Twitter")]
        Tweet = 39,

        [Description("Facebook")]
        ProfileScraper = 40,

        [Description("Quora")]
        DownvoteQuestions = 41,

        [Description("Quora")]
        UpvoteAnswers = 42,

        [Description("Gplus")]
        Join = 43,

        [Description("Gplus")]
        Unjoin = 44,

        [Description("Facebook")]
        PostLikerCommentor = 45,

        [Description("Facebook")]
        FanpageLiker = 46,

        [Description("Facebook")]
        WebpageLikerCommentor = 47,

        [Description("Twitter")]
        TweetScraper = 48,

        [Description("Facebook")]
        MakeAdmin = 49,

        [Description("LinkedIn")]
        ConnectionRequest = 50,

        [Description("YouTube")]
        Subscribe = 51,

        [Description("Facebook,Youtube")]
        Share = 52,

        [Description("YouTube")]
        LikeComments = 53,

        [Description("YouTube")]
        UnSubscribe = 54,

        [Description("YouTube")]
        ViewIncreaser = 55,

        [Description("Instagram")]
        BlockFollower = 56,

        [Description("Instagram")]
        LikeComment = 57,

        [Description("Instagram")]
        HashtagsScraper = 58,

        [Description("Pinterest")]
        CreateBoard = 59,

        [Description("Twitter,Instagram")]
        FollowBack = 60,

        [Description("Twitter")]
        DeleteTweet = 61,

        [Description("Twitter")]
        Mute = 62,

        [Description("Gplus")]
        CommunityScraper = 63,


        [Description("LinkedIn")]
        JobScraper = 64,

        [Description("LinkedIn")]
        CompanyScraper = 65,

        [Description("LinkedIn")]
        GroupMemberScraper = 66,

        [Description("LinkedIn")]
        SalesNavigatorUserScraper = 67,


    }
}
