using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class InteractedPosts
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        //ID of the Pin
        [Column(Order = 2)]
        public string PinId { get; set; }

        //ID/Path of the media file
        [Column(Order = 3)]
        public string MediaString { get; set; }
        /// <summary>
        /// Message/Description of the Pin
        /// </summary>
        /// 

        [Column(Order = 4)]
        public string PinDescription { get; set; }

        //Like Count Of The Pin
        [Column(Order = 5)]
        public int TryCount { get; set; }

        //Comment Count Of The Pin
        [Column(Order = 6)]
        public int CommentCount { get; set; }


        //Time when the Pin has been posted in TimeStamp
        [Column(Order = 7)]
        public double PinnedTimeStamp { get; set; }

        //Duration of the video Pins
        [Column(Order = 8)]
        public double VideoDuration
        { get; set; }

        //ID of the Board
        [Column(Order = 9)]
        public string BoardId { get; set; }

        //Web url of the Pin
        [Column(Order = 10)]
        public string PinWebUrl { get; set; }

        // Board Name in which the Pin belongs to
        [Column(Order = 11)]
        public string BoardName { get; set; }

        [Column(Order = 12)]
        public int InteractionDate { get; set; }

        //Type of the Media(Image/Video)
        [Column(Order = 13)]
        public MediaType MediaType { get; set; }

        //Type of Operation performed(follow/comment...etc)
        [Column(Order = 14)]
        public ActivityType OperationType { get; set; }

        //User id of the User in which the Pin belongs to
        [Column(Order = 15)]
        public string UserId { get; set; }

        //Username of the User in which the Pin belongs to
        [Column(Order = 16)]
        public string Username { get; set; }

        [Column(Order = 17)]
        public string Query { get; set; }

        [Column(Order = 18)]
        public string QueryType
        { get; set; }



    }
}
