using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Campaigns
{
    public class InteractedUsers
    {
        [Column(Order = 1)]
        public string Query { get; set; }

        [Column(Order = 2)]
        public string QueryType
        { get; set; }

        [Column(Order = 3)]
        public int FollowedBack
        { get; set; }

        [Column(Order = 4)]
        public int FollowedBackDate
        { get; set; }

        [Column(Order = 5)]
        public int InteractionTime
        { get; set; }

        [Column(Order = 6)]
        public string ActivityType
        { get; set; }

        [Column(Order = 7)]
        public string Username
        { get; set; }

        [Column(Order = 8)]
        public string InteractedUsername
        { get; set; }

        [Column(Order = 9)]
        public string DirectMessage
        { get; set; }

        [Column(Order = 10)]
        public string InteractedUserId
        { get; set; }

        [Column(Order = 11)]
        public int UpdatedTime
        { get; set; }

        [Column(Order = 12)]
        public int FollowersCount
        { get; set; }


        [Column(Order = 13)]
        public int FollowingsCount
        { get; set; }

        [Column(Order = 14)]
        public int PinsCount
        { get; set; }

        [Column(Order = 15)]
        public int TriesCount { get; set; }

        [Column(Order = 16)]
        public string FullName
        { get; set; }


        [Column(Order = 17)]
        public bool? HasAnonymousProfilePicture
        { get; set; }


        [Column(Order = 18)]
        public bool IsVerified
        { get; set; }

        [Column(Order = 19)]
        public string ProfilePicUrl
        { get; set; }

        [Column(Order = 20)]
        public string Website { get; set; }

        [Column(Order = 21)]
        public string Bio { get; set; }

        [Column(Order = 22)]
        public string SinAccId { get; set; }

        [Column(Order = 23)]
        public string SinAccUsername { get; set; }

        [Column(Order = 24)]
        public DateTime InteractionDateTime { get; set; }
        [Column(Order = 25)]
        public int InteractionTimeStamp { get; set; }

    }
}
