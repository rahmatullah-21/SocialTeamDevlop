using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.GplusTables.Accounts
{
    public class Friendships
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
        public string ActivityType
        { get; set; }

        [Column(Order = 5)]
        public string UserId
        { get; set; }

        [Column(Order = 6)]
        public string FullName
        { get; set; }

        [Column(Order = 7)]
        public string ProfileUrl
        { get; set; }


        [Column(Order = 8)]
        public int Time
        { get; set; }

        [Column(Order = 9)]
        //public  FollowType FollowType
        public int FollowType
        { get; set; }

        [Column(Order = 10)]
        public int FollowedMeBack
        { get; set; }

        [Column(Order = 11)]
        public int BlockedStatus
        { get; set; }

        [Column(Order = 12)]
        public int ActiveStatus
        { get; set; }


        [Column(Order = 13)]
        public int FollowerCount
        { get; set; }
       

        [Column(Order = 14)]
        public string Biography
        { get; set; }

        [Column(Order = 15)]
        public int Gender
        { get; set; }

        [Column(Order = 16)]
        public bool? HasAnonymousProfilePicture
        { get; set; }

        [Column(Order = 17)]
        public string ProfilePicUrl
        { get; set; }


        [Column(Order = 18)]
        public int IsVerified
        { get; set; }




    }

    //public enum FollowType : int
    //{
    //    Following, NotFollowing, Unfollowed
    //}       
}
