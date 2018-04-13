using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.TdTables.Accounts
{
    public class Friendships
    {
        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }


        [Column(Order = 2)]
        public int DetailedInfoHasBeenRetrievedAtleastOnce
        { get; set; }


        [Column(Order = 3)]
        public int DetailedInfoWillNotBeRetrieved
        { get; set; }

        [Column(Order = 4)]
        public string Username
        { get; set; }


        [Column(Order = 5)]
        [Unique]
        public string UserId
        { get; set; }

        [Column(Order = 6)]
        public string FullName
        { get; set; }


        [Column(Order = 7)]
        public int FollowersCount
        { get; set; }


        [Column(Order = 8)]
        public int FollowingsCount
        { get; set; }



        [Column(Order = 9)]
        public int TweetsCount
        { get; set; }

        [Column(Order = 10)]
        public int LikesCount { get; set; }

        [Column(Order = 11)]
        public int  HasAnonymousProfilePicture { get; set; }

        [Column(Order = 12)]
        public int IsPrivate { get; set; }

        [Column(Order = 13)]
        public int IsVerified { get; set; }

        [Column(Order = 14)]
        public string ProfilePicUrl { get; set; }

        [Column(Order = 15)]
        public int Time { get; set; }

        [Column(Order = 16)]
        public FollowType FollowType
        { get; set; }

        [Column(Order = 17)]
        public int JoinedDate { get; set; }

        [Column(Order = 18)]
        public string Location { get; set; }

        [Column(Order = 19)]
        public string Website { get; set; }

        [Column(Order = 20)]
        public string Bio { get; set; }

        [Column(Order = 21)]
        public int IsMuted { get; set; }
    }

    
    public enum FollowType : int
    {
        Following = 1,
        Followers  = 2,
        Mutual = 3
    }       
}
