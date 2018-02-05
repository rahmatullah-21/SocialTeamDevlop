namespace DominatorHouseCore.Enums
{
    public enum SocialNetworks
    {
        Facebook,
        Instagram,
        Twitter,
        PinInterest,
        LinkedIn,
        Reddit,
        Craglist,
        Backpage
    }

    public enum ActivityType
    {
        Follow = 1,
        Unfollow = 2,
        Like = 3,
        Unlike = 4,
        Comment = 5,
        DeleteComment = 6,
        Post = 7,
        Repost = 8,
        DeletePost = 9,
        Message = 10,
        UserScraper = 11,
        DownloadScraper = 12,
        Reposter = 13,
        Retweet = 14


    }

    public enum DatabaseType
    {
        CampaignType=1,
        AccountType=2,
        GlobalType =3
    }

}