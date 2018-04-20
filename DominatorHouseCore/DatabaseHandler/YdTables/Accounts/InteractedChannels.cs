using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.YdTables.Accounts
{
    public class InteractedChannels
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
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
        public string InteractedChannelName { get; set; }

        [Column(Order = 7)]
        public string InteractedChannelId { get; set; }

        [Column(Order = 8)]
        public int InteractionTimeStamp { get; set; }

        [Column(Order = 9)]
        public string SubscriberCount { get; set; }

        [Column(Order = 10)]
        public string SubscribeStatus { get; set; }

        [Column(Order = 11)]
        public string ViewsCount { get; set; }

        [Column(Order = 12)]
        public string ChannelProfilePic { get; set; }

        [Column(Order = 13)]
        public string ChannelLocation { get; set; }

        [Column(Order = 14)]
        public string VideosCount { get; set; }

        [Column(Order = 15)]
        public string ChannelDescription { get; set; }

        [Column(Order = 16)]
        public string ChannelJoinedDate { get; set; }

        [Column(Order = 17)]
        public string ExternalLinks { get; set; }

        [Column(Order = 18)]
        public string ChannelUrl { get; set; }

        [Column(Order = 19)]
        public string MessageToChannelOwner { get; set; }
    }
}
