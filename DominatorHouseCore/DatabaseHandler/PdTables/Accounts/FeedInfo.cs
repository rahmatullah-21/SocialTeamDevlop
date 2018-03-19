using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.PdTables.Accounts
{
    public class FeedInfoes
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]


        //Row ID
        public int Id { get; set; }
        
        //ID of the tweet
        [Column(Order = 2)]
        public string PinId { get; set; }

        //ID/Path of the media file
        [Column(Order = 3)]
        public string MediaString { get; set; }
        /// <summary>
        /// Message/Description of the tweet
        /// </summary>
        /// 

        [Column(Order = 4)]
        public string PinDescription { get; set; }

        //Try Count Of The Tweet
        [Column(Order = 5)]
        public int TryCount { get; set; }
      

        //Comment Count Of The Tweet
        [Column(Order = 6)]
        public int CommentCount { get; set; }



        //Time when the Pin has been posted in TimeStamp
        [Column(Order = 7)]
        public double PinnedTimeStamp { get; set; }

        //Duration of the video Pins
        [Column(Order = 8)]
        public double VideoDuration
        { get; set; }

        [Column(Order = 9)]
        public string BoardId { get; set; }

        [Column(Order = 10)]
        public string PinWebUrl { get; set; }

        [Column(Order = 11)]
        public string BoardName { get; set; }

    }
}
