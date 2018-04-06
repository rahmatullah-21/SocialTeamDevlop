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
        public string UserId
        { get; set; }

        [Column(Order = 3)]
        public string FullName
        { get; set; }


        [Column(Order = 4)]
        public bool? HasAnonymousProfilePicture
        { get; set; }


        [Column(Order = 5)]
        public bool IsVerified
        { get; set; }


        [Column(Order = 6)]
        public string ProfilePicUrl
        { get; set; }

        [Column(Order = 7)]
        public int Time
        { get; set; }

        [Column(Order = 8)]
        public  FollowType FollowType
        { get; set; }

        [Column(Order = 9)]
        public string InteractedUserId
        { get; set; }


    }

    public enum FollowType : int
    {
        Following, NotFollowing, Unfollowed
    }       
}
