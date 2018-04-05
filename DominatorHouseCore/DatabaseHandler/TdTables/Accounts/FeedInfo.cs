using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.TdTables.Accounts
{
    public class FeedInfoes
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }
        
        /// <summary>
        /// Id of the Tweet
        /// </summary>
        [Column(Order = 2)]
        public string TweetId { get; set; }

        /// <summary>
        ///  path of the media
        /// </summary>
        [Column(Order = 3)]
        public string MediaId { get; set; }
        /// <summary>
        /// Description of the tweet
        /// </summary>

        [Column(Order = 4)]
        public string TwtMessage { get; set; }

        /// <summary>
        /// Like Count Of The Tweet
        /// </summary>
        
        [Column(Order = 5)]
        public int LikeCount { get; set; }

        /// <summary>
        /// Retweet Count Of The Tweet
        /// </summary>
        
        [Column(Order = 6)]
        public int RetweetCount { get; set; }

        /// <summary>
        /// Comment Count Of The Tweet
        /// </summary>
        
        [Column(Order = 7)]
        public int CommentCount { get; set; }

        /// <summary>
        /// True if the tweet has been retweeted
        /// </summary>
        
        [Column(Order = 8)]
        public bool IsRetweet { get; set; }

        /// <summary>
        /// Time when the tweet has been posted in TimeStamp
        /// </summary>
        
        [Column(Order = 9)]
        public double TweetedTimeStamp { get; set; }

        /// <summary>
        /// Duration of the video tweets
        /// </summary>

        [Column(Order = 10)]
        public double VideoDuration
        { get; set; }

        /// <summary>
        /// View count of the video
        /// </summary>
        
        [Column(Order = 11)]
        public int ViewCount
        { get; set; }

        [Column(Order = 12)]
        public int IsComment { get; set; }

        /// <summary>
        /// tweet id on which comment is done
        /// </summary>

        [Column(Order = 13)]
        public string CommentedOnTweetID
        { get; set; }

        /// <summary>
        /// owner of the tweet on which comment or retweet is done
        /// </summary>

        [Column(Order = 14)]
        public string TweetOwnerIfRetweetedOrCommented
        { get; set; }

    }
}
