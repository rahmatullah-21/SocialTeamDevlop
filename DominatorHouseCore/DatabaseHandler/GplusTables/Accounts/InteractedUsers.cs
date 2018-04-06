using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class InteractedUsers
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }

        [Column(Order = 2)]
        public string Query { get; set; }

        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        [Column(Order = 4)]
        public int FollowedBack
        { get; set; }

        [Column(Order = 5)]
        public int Date
        { get; set; }

        [Column(Order = 6)]
        public string ActivityType
        { get; set; }

        [Column(Order = 7)]
        public string UserId
        { get; set; }

        [Column(Order = 8)]
        public string InteractedUserId
        { get; set; }

        [Column(Order = 9)]
        public string InteractedUserName
        { get; set; }

        [Column(Order = 10)]
        public int Time
        { get; set; }

        [Column(Order = 11)]
        public string FullName
        { get; set; }


        [Column(Order = 12)]
        public bool? HasAnonymousProfilePicture
        { get; set; }


        [Column(Order = 13)]
        public bool IsVerified
        { get; set; }


        [Column(Order = 14)]
        public string ProfilePicUrl
        { get; set; }

       
        [Column(Order = 15)]
        public FollowType FollowType
        { get; set; }
    }
}
