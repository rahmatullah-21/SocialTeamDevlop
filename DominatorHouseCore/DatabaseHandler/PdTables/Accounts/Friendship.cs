using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
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
        public int Followers
        { get; set; }


        [Column(Order = 5)]
        public int Followings
        { get; set; }



        [Column(Order = 6)]
        public int PinsCount
        { get; set; }

        [Column(Order = 7)]
        public int BoardsCount { get; set; }


        [Column(Order = 8)]
        public string FullName
        { get; set; }


        [Column(Order = 9)]
        public bool ? HasAnonymousProfilePicture
        { get; set; }



        [Column(Order = 10)]
        public bool IsPrivate
        { get; set; }


        [Column(Order = 11)]
        public bool IsVerified
        { get; set; }

  
        [Column(Order = 12)]
        public string ProfilePicUrl
        { get; set; }




        [Column(Order = 13)]
        public string Username
        { get; set; }


        [Column(Order = 14)]
        public string UserId
        { get; set; }



        [Column(Order = 15)]
        public int Time
        { get; set; }


        [Column(Order = 16)]
        public FollowType FollowType
        { get; set; }

        [Column(Order = 17)]
        public string Website { get; set; }

        [Column(Order = 18)]
        public string Bio { get; set; }
    }

    [Flags]
    public enum FollowType : int
    {
        Following = 1,
        FollowingBack  = 2,
        Pending = 4
    }       
}
