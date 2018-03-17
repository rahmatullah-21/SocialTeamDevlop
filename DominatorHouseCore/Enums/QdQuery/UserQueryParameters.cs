using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{   
    public enum UserQueryParameters
    {
        [Description("langKeywords")]
        Keywords,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers,
        [Description("langSomeonesFollowings")]
        SomeonesFollowings,
        [Description("langCustomUsers")]
        CustomUsers,
        [Description("langEngagedUsers")]
        EngagedUsers,
        [Description("langOwnFollowers")]
        OwnFollowers,
        [Description("langTopicFollowers")]
        TopicFollowers,
        [Description("langAnswerUpvoters")]
        AnswerUpvoters
    }
}