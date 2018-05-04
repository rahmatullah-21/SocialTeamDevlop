using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{   
    public enum UserQueryParameters
    {
        [Description("QDlangKeywords")]
        Keywords,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers,
        [Description("QDlangSomeonesFollowings")]
        SomeonesFollowings,
        [Description("QDlangCustomUsers")]
        CustomUsers,
        [Description("QDlangEngagedUsers")]
        EngagedUsers,
        //[Description("QDlangOwnFollowers")]
        //OwnFollowers,
        [Description("QDlangTopicFollowers")]
        TopicFollowers,
        [Description("QDlangAnswerUpvoters")]
        AnswerUpvoters
    }

    
}