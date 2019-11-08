using DominatorHouseCore.DatabaseHandler.Common;

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Campaigns
{
    public class InteractedUsers : Entity
    {
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string Query { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public int FollowedBack
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public int FollowedBackDate
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int Date { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ActivityType
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string AccountUsername
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string InteractedUsername
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string InteractedUserId
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string RequiredData
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string Gender
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string Status
        { get; set; }
    }
}
