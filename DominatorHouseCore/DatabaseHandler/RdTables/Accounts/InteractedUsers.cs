using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.RdTables.Accounts
{
    public class InteractedUsers
    {
        [Column(Order = 1)]
        public string Query { get; set; }

        [Column(Order = 2)]
        public string QueryType { get; set; }

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
        public int FollowersCount
        { get; set; }

        [Column(Order = 13)]
        public int FollowingsCount
        { get; set; }

        [Column(Order = 14)]
        public int RedditCount
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
        public string ActivityType { get; set; }
    }
}
