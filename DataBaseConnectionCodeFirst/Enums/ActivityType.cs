using System.ComponentModel;

public enum ActivityType
{
    [Description("Twitter,Instagram")]
    Follow = 1,

    [Description("Twitter,Instagram")]
    Unfollow = 2,

    [Description("Facebook,Twitter,Instagram")]
    Like = 3,

    [Description("Twitter,Instagram")]
    Unlike = 4,

    [Description("Facebook,Twitter,Instagram")]
    Comment = 5,

    [Description("Twitter,Instagram")]
    DeleteComment = 6,

    [Description("Facebook,Twitter,Instagram")]
    Post = 7,

    [Description("Twitter,Instagram")]
    Repost = 8,

    [Description("Facebook,Twitter,Instagram")]
    DeletePost = 9,

    [Description("Facebook,Twitter,Instagram")]
    Message = 10,

    [Description("Facebook,Twitter,Instagram")]
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
    ReportUsers = 21
}
