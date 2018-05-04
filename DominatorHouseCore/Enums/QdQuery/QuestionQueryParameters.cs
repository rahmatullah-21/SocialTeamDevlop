using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum QuestionQueryParameters
    {
        [Description("QDlangCustomURL")]
        CustomUrl,
        [Description("QDlangKeywords")]
        Keywords,
        [Description("QDlangCustomUser")]
        CustomUser,
        [Description("QDlangTopicList")]
        TopicFaqs
    }
}