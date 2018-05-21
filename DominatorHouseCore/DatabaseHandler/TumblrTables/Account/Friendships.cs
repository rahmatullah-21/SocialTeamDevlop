using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.TumblrTables.Account
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
        public int PostsPerWeek
        { get; set; }

        [Column(Order = 7)]
        public int Uploads
        { get; set; }


        [Column(Order = 8)]
        public string FullName
        { get; set; }


        [Column(Order = 9)]
        public bool? HasAnonymousProfilePicture
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
        // [Unique]
        public string Username
        { get; set; }



        [Column(Order = 14)]
        public bool IsBusiness
        { get; set; }


        [Column(Order = 15)]
        public string UserId
        { get; set; }



        [Column(Order = 16)]
        public int Time
        { get; set; }


        [Column(Order = 17)]
        public FollowType FollowType
        { get; set; }

        [Column(Order = 18)]
        public bool IsFollowBySoftware
        { get; set; }
    }
}
