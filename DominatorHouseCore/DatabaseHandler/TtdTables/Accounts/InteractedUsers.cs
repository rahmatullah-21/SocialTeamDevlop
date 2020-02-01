using DominatorHouseCore.DatabaseHandler.Common;

namespace DominatorHouseCore.DatabaseHandler.TtdTables.Accounts
{
    public class InteractedUsers: Entity
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
        public int Gender
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string FullName
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string ProfilePicUrl
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public bool IsVerified
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public bool IsBlocked
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public bool IsBlocking
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string Birthday
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string Signature
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string FollowingsCount
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string FollowersCount
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string FeedsCount
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string Country
        { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string Status
        { get; set; }
    }
}
