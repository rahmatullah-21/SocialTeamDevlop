using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.YdTables.Campaign
{
    public class InteractedPosts
    {
        [PrimaryKey]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [Indexed]
        [AutoIncrement]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        public string AccountUsername { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        public string QueryType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        public string QueryValue { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        public string ActivityType { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 7)]
        public string ChannelName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 8)]
        public string ChannelId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 9)]
        public string ChannelUserName { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 10)]
        public string ChannelUserId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 11)]
        public string ViewsCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 12)]
        public string LikeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 13)]
        public string DislikeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 14)]
        public string LikeStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 15)]
        public string PublishedDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 16)]
        public string PostDescription { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 17)]
        public string SubscribeCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 18)]
        public string CommentCount { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 19)]
        public string VideoUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 20)]
        public string VideoDuration { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 21)]
        public string SubscribeStatus { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 22)]
        public string CommentsPresent { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 23)]
        public string CommentToVideo { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 24)]
        public string ReplyToComment { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 25)]
        public string CommentId { get; set; }
    }
}
