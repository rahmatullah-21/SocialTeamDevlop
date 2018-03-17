using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum AnswerQueryParameters
    {
        [Description("langCustomURL")]
        CustomUrl,
        [Description("langKeywords")]
        Keywords,
        [Description("langCustomUser")]
        CustomUser,
        [Description("langTopicList")]
        TopicList
    }
}