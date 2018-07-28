using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.YdTables.Campaign
{
    public class InteractedPosts
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string AccountUsername { get; set; }

        [Column(Order = 3)]
        public string QueryType { get; set; }

        [Column(Order = 4)]
        public string QueryValue { get; set; }

        [Column(Order = 5)]
        public string ActivityType { get; set; }

        [Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }

        [Column(Order = 7)]
        public string ChannelName { get; set; }

        [Column(Order = 8)]
        public string ChannelId { get; set; }

        [Column(Order = 9)]
        public string ChannelUserName { get; set; }

        [Column(Order = 10)]
        public string ChannelUserId { get; set; }

        [Column(Order = 11)]
        public string ViewsCount { get; set; }

        [Column(Order = 12)]
        public string LikeCount { get; set; }

        [Column(Order = 13)]
        public string DislikeCount { get; set; }

        [Column(Order = 14)]
        public string LikeStatus { get; set; }

        [Column(Order = 15)]
        public string PublishedDate { get; set; }

        [Column(Order = 16)]
        public string PostDescription { get; set; }

        [Column(Order = 17)]
        public string SubscribeCount { get; set; }

        [Column(Order = 18)]
        public string CommentCount { get; set; }

        [Column(Order = 19)]
        public string VideoUrl { get; set; }

        [Column(Order = 20)]
        public string VideoDuration { get; set; }

        [Column(Order = 21)]
        public string SubscribeStatus { get; set; }

        [Column(Order = 22)]
        public string CommentsPresent { get; set; }

        [Column(Order = 23)]
        public string CommentToVideo { get; set; }

        [Column(Order = 24)]
        public string ReplyToComment { get; set; }

        [Column(Order = 25)]
        public string CommentId { get; set; }
    }
}
