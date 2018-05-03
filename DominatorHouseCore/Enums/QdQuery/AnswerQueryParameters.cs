using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum AnswerQueryParameters
    {
        [Description("QDlangCustomURL")]
        CustomUrl,
        [Description("QDlangKeywords")]
        Keywords,
        [Description("QDlangCustomUser")]
        CustomUser,
        [Description("QDlangTopicList")]
        TopicList
    }
}