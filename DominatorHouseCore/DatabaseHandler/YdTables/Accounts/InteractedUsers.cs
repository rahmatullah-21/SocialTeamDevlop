using DominatorHouseCore.DatabaseHandler.Common;

namespace DominatorHouseCore.DatabaseHandler.YdTables.Accounts
{
    public class InteractedUsers : Entity
    {
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public string InteractedUserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string InteractedUserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string SubscriberCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string SubscribeStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string ViewsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string UserProfilePic { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string UserLocation { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string VideosCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string UserDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string UserJoinedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string ExternalLinks { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string UserUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string MessageToChannelOwner { get; set; }
    }
}
