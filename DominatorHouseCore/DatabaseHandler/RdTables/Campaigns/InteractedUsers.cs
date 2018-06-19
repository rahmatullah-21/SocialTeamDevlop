using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Campaigns
{
    public class InteractedUsers
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }
        
        [Column(Order = 2)]
        public string QueryType { get; set; }

        [Column(Order = 3)]
        public string QueryValue { get; set; }

        [Column(Order = 4)]
        public string ActivityType { get; set; }

        [Column(Order = 5)]
        public int InteractionTime
        { get; set; }

        [Column(Order = 6)]
        public string Username
        { get; set; }

        [Column(Order = 7)]
        public string InteractedUsername
        { get; set; }

        [Column(Order = 9)]
        public int Date
        { get; set; }

        [Column(Order = 10)]
        public string InteractedUserId
        { get; set; }

        [Column(Order = 11)]
        public int UpdatedTime
        { get; set; }
        [Column(Order = 12)]
        public string accountIcon
        { get; set; }
        [Column(Order = 13)]
        public int commentKarma
        { get; set; }
        [Column(Order = 14)]
        public long created
        { get; set; }
        [Column(Order = 15)]
        public string displayName
        { get; set; }
        [Column(Order = 16)]
        public string displayNamePrefixed
        { get; set; }
        [Column(Order = 17)]
        public string displayText
        { get; set; }
        [Column(Order = 18)]
        public bool hasUserProfile
        { get; set; }
        [Column(Order = 19)]
        public bool isEmployee
        { get; set; }
        [Column(Order = 20)]
        public bool isFollowing
        { get; set; }
        [Column(Order = 21)]
        public bool isGold
        { get; set; }
        [Column(Order = 22)]
        public bool isMod
        { get; set; }
        [Column(Order = 23)]
        public bool isNSFW
        { get; set; }
        [Column(Order = 24)]
        public bool prefShowSnoovatar
        { get; set; }
        [Column(Order = 25)]
        public int postKarma
        { get; set; }
        [Column(Order = 26)]
        public string url
        { get; set; }
       
        [Column(Order = 27)]
        public string SinAccId { get; set; }

        [Column(Order = 28)]
        public string SinAccUsername { get; set; }

        [Column(Order = 29)]
        public DateTime InteractionDateTime { get; set; }
        [Column(Order = 30)]
        public int InteractionTimeStamp { get; set; }
        [Column(Order = 31)]
        public int FollowedBack
        { get; set; }

        [Column(Order = 32)]
        public int FollowedBackDate
        { get; set; }

    }
}
