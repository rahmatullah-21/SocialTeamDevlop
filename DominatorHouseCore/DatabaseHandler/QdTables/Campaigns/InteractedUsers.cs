using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;
namespace DominatorHouseCore.DatabaseHandler.QdTables.Campaigns
{
    public class InteractedUsers
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string SinAccUsername { get; set; }

        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        [Column(Order = 4)]
        public string QueryValue { get; set; }

        [Column(Order = 5)]
        public string ActivityType
        { get; set; }


        [Column(Order = 6)]
        public string InteractedUsername
        { get; set; }

        [Column(Order = 7)]
        public string InteractedUserId
        { get; set; }

        [Column(Order =8)]
        public string InteractedUserFullName
        { get; set; }

        [Column(Order = 9)]
        public int FollowStatus
        { get; set; }

        [Column(Order = 10)]
        public int FollowBackStatus
        { get; set; }

        [Column(Order = 11)]
        public int InteractionTimeStamp
        { get; set; }

        [Column(Order = 12)]
        public string DirectMessage
        { get; set; }


        [Column(Order = 13)]
        public int UpdatedTime
        { get; set; }

        [Column(Order = 14)]
        public int FollowersCount
        { get; set; }

        [Column(Order = 15)]
        public int FollowingsCount
        { get; set; }

        [Column(Order = 16)]
        public int TweetsCount
        { get; set; }

        [Column(Order = 17)]
        public int LikesCount { get; set; }

        [Column(Order = 18)]
        public int HasAnonymousProfilePicture
        { get; set; }

        [Column(Order = 19)]
        public int IsPrivate
        { get; set; }



        [Column(Order = 20)]
        public string ProfilePicUrl
        { get; set; }


        [Column(Order = 21)]
        public int JoinedDate { get; set; }

        [Column(Order = 22)]
        public string Location { get; set; }

        [Column(Order = 23)]
        public string Website { get; set; }

        [Column(Order = 24)]
        public string Bio { get; set; }

        /// <summary>
        /// Describes wheather the activity is done in Activity process or after activity process
        /// </summary>
        [Column(Order = 25)]
        public string ProcessType { get; set; }

        [Column(Order = 26)]
        public DateTime InteractionDateTime { get; set; }

    }
}
