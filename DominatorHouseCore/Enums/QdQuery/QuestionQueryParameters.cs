using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum QuestionQueryParameters
    {
        [Description("langCustomURL")]
        CustomUrl,
        [Description("langKeywords")]
        Keywords,
        [Description("langCustomUser")]
        CustomUser,
        [Description("langTopicFaqs")]
        TopicFaqs
    }
}