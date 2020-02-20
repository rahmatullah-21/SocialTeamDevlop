using System.ComponentModel;

namespace DominatorHouseCore.Enums
{
    public enum ActivityType
    {
        [Description("Twitter,Instagram,Gplus,Quora,Tumblr,Pinterest,Reddit,TikTok")]
        Follow = 1,

        [Description("Twitter,Instagram,Gplus,Quora,Tumblr,Pinterest,Reddit,TikTok")]
        Unfollow = 2,

        [Description("Twitter,Instagram,Gplus,Tumblr,LinkedIn,Youtube,TikTok")]
        Like = 3,

        [Description("Twitter,Instagram,Gplus,Tumblr")]
        Unlike = 4,

        [Description("Twitter,Instagram,Gplus,Pinterest,LinkedIn,Tumblr,Youtube,Reddit,TikTok")]
        Comment = 5,

        //,Instagram,Reddit
       // [Description("Twitter")]
        DeleteComment = 6,
    

        //[Description("Instagram")]
        Post = 7,

        // INFO : Ambiguous between Repost(8) and Reposter(13), so dont use, if any one used please with Reposter(13)
       // [Description("Twitter")]
       // Repost = 8,

        [Description("Instagram")]
        DeletePost = 9,

        //[Description("Facebook,Twitter,Instagram")]
        //Message = 10,

        [Description("Twitter,Instagram,Gplus,LinkedIn,Pinterest,Reddit,Tumblr,Quora,TikTok")]
        UserScraper = 11,

         [Description("Facebook")]
         DownloadScraper = 12,

        [Description("Twitter,Instagram")]
        Reposter = 13,

        [Description("Twitter")]
        Retweet = 14,

        [Description("Quora")]
        QuestionsScraper = 15,

        [Description("Quora")]
        AnswersScraper = 16,

        //[Description("Quora")]
        //VoteAnswers = 17,

        [Description("Quora")]
        DownvoteAnswers = 18,

        //[Description("Quora")]
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
        WithdrawSentRequest = 25,

        [Description("Facebook")]
        Unfriend = 26,

        [Description("Facebook")]
        GroupScraper = 27,

        [Description("Facebook")]
        FanpageScraper = 28,

        [Description("Facebook,Gplus,Instagram,Reddit,Tumblr")]
        CommentScraper = 29,

        [Description("Facebook,Gplus,Youtube,Instagram,Tumblr,TikTok")]
        PostScraper = 30,

        [Description("Facebook,LinkedIn,LangKeyGroupJoiner")]
        GroupJoiner = 31,

        [Description("Facebook,LinkedIn,LangKeyGroupUnjoiner")]
        GroupUnJoiner = 32,

        [Description("Facebook")]
        GroupInviter = 34,

        [Description("Facebook")]
        PageInviter = 35,

        [Description("Facebook")]
        EventInviter = 36,

        //[Description("Facebook")]
        //GroupCreator = 37,

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

        //[Description("Gplus")]
        //Join = 43,

        //[Description("Gplus")]
        //Unjoin = 44,

       // [Description("Facebook")]
        PostLikerCommentor = 45,

        [Description("Facebook")]
        FanpageLiker = 46,

        //[Description("Facebook")]
        WebpageLikerCommentor = 47,

        [Description("Twitter")]
        TweetScraper = 48,

        [Description("Facebook")]
        MakeAdmin = 49,

        [Description("LinkedIn")]
        ConnectionRequest = 50,

        [Description("Youtube,Reddit")]
        Subscribe = 51,

        [Description("LinkedIn")]
        Share = 52,

        //[Description("Youtube")]
        //LikeComments = 53,

        [Description("Reddit,Youtube")]
        UnSubscribe = 54,

        [Description("Youtube")]
        ViewIncreaser = 55,

        [Description("Instagram")]
        BlockFollower = 56,
        [Description("Instagram,Youtube,Facebook")]
        LikeComment = 57,

        [Description("Instagram,TikTok")]
        HashtagsScraper = 58,

        [Description("Pinterest")]
        CreateBoard = 59,

        [Description("Twitter,Instagram,Pinterest")]
        FollowBack = 60,

        //[Description("Twitter")]
        //DeleteTweet = 61,

        [Description("Twitter")]
        Mute = 62,

        //[Description("Gplus")]
       // CommunityScraper = 63,

        [Description("LinkedIn")]
        JobScraper = 64,

        [Description("LinkedIn")]
        CompanyScraper = 65,

        //[Description("LinkedIn")]
        //GroupMemberScraper = 66,

        [Description("LinkedIn")]
        SalesNavigatorUserScraper = 67,

        [Description("Youtube,Reddit")]
        ChannelScraper = 68,

       // [Description("Youtube")]
        Unsubscribe = 69,

        [Description("Instagram,Twitter,Facebook,LinkedIn,Pinterest,Quora,Tumblr")]
        BroadcastMessages = 70,

        [Description("Instagram,Twitter,Pinterest,Quora")]
        SendMessageToFollower = 71,

        [Description("Instagram,Twitter,Facebook,LinkedIn,Pinterest,Quora")]
        AutoReplyToNewMessage = 72,

        [Description("LinkedIn")]
        AcceptConnectionRequest = 73,

        [Description("LinkedIn")]
        RemoveConnections = 74,

        [Description("LinkedIn")]
        ProfileEndorsement = 75,

        //[Description("Youtube")]
        //LikeToComment = 76,

        //[Description("Youtube")]
        //DislikeToComment = 77,

        [Description("Facebook,Instagram")]
        ReplyToComment = 78,

        [Description("Tumblr")]
        Reblog = 79,

        [Description("Pinterest")]
        Try = 80,

        [Description("LinkedIn")]
        SendMessageToNewConnection = 81,

        [Description("Reddit")]
        UrlScraper = 82,

        [Description("LinkedIn")]
        SendGreetingsToConnections = 83,

        [Description("Reddit")]
        Reply = 84,

        [Description("Twitter")]
        Delete = 85,

        [Description("Facebook")]
        IncommingFriendRequest = 86,

        [Description("Reddit")]
        Downvote = 87,

        [Description("Reddit")]
        Upvote = 88,

        //[Description("Reddit")]
        //SubredditScraper = 89,

        [Description("Pinterest")]
        DeletePin = 90,

        [Description("Pinterest")]
        Repin = 91,

        [Description("LinkedIn")]
        WithdrawConnectionRequest = 92,

        [Description("Reddit")]
        RemoveVote = 93,
        [Description("LinkedIn")]
        ExportConnection = 94,

        [Description("Facebook,LangKeyPostLikers")]
        PostLiker = 95,

        [Description("Facebook,LangKeyPostComment")]
        PostCommentor = 96,
        [Description("Reddit")]
        RemoveVoteComment = 97,
        [Description("Reddit")]
        UpvoteComment = 98,
        [Description("Reddit")]
        DownvoteComment = 99,
        [Description("Youtube")]
        Dislike = 100,
        [Description("Quora")]
        AnswerOnQuestions = 101,
        [Description("Twitter")]
        WelcomeTweet = 102,
        [Description("LinkedIn")]
        SalesNavigatorCompanyScraper = 103,
        [Description("Twitter")]
        TweetTo = 104,
        [Description("Facebook")]
        SendMessageToNewFriends = 105,
        [Description("Facebook")]
        WatchPartyInviter = 106,
       // [Description("Facebook")]
        MarketPlaceScraper = 107,
        [Description("Facebook")]
        SendGreetingsToFriends = 108,
        //[Description("Facebook")]
        WebPostLikeComment = 109,
        [Description("Pinterest")]
        EditPin = 110,
        [Description("Pinterest")]
        AcceptBoardInvitation = 111,
        [Description("Pinterest")]
        SendBoardInvitation = 112,
        [Description("LinkedIn")]
        BlockUser = 113,
        [Description("Facebook")]
        MessageToFanpages = 114,
        [Description("Facebook")]
        MessageToPlaces = 115,
        [Description("Facebook")]
        PlaceScraper = 116,
        [Description("Reddit")]
        EditComment = 117,
        [Description("Instagram")]
        StoryViewer = 118,
        [Description("LinkedIn,LangKeyDeleteConversations")]
        DeleteConversations = 119,
        [Description("Facebook")]
        CommentRepliesScraper = 120,
        [Description("LinkedIn")]
        MessageConversationScraper =121,
        [Description("LinkedIn")]
        FollowPages = 122,
        [Description("LangKeyStopAll")]
        StopAll = 123
    }
}
