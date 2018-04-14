using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DominatorHouseCore.Enums;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.YdTables.Campaign
{
    public class InteractedPosts
    {
        [Key]
        [Column(Order = 1)]
        [Index]
        [Autoincrement]
        public int Id { get; set; }

        /// <summary>
        /// UserName of the Account from which Interaction is done
        /// </summary>

        [Column(Order = 2)]
        public string SinAccUsername { get; set; }

        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        [Column(Order = 4)]
        public string QueryValue { get; set; }

        [Column(Order = 5)]
        public string ActivityType { get; set; }

        [Column(Order = 6)]
        public int InteractionTimeStamp { get; set; }

        /// <summary>
        /// Id of the tweet
        /// </summary>

        [Column(Order = 7)]
        public string Channel { get; set; }

        /// <summary>
        /// UserId of the tweet Owner
        /// </summary>

        [Column(Order = 8)]
        public string UserId { get; set; }

        /// <summary>
        /// UserName of the tweet owner
        /// </summary>

        [Column(Order = 9)]
        public string Username { get; set; }

        /// <summary>
        ///Image/Video url of tweet
        /// </summary>

        [Column(Order = 10)]
        public string MediaId { get; set; }

        /// <summary>
        /// Message/Description of the tweet
        /// </summary>

        [Column(Order = 11)]
        public string ChannelUrl { get; set; }

        /// <summary>
        /// Like Count Of The Tweet
        /// </summary>

        [Column(Order = 12)]
        public int LikeCount { get; set; }

        /// <summary>
        /// Retweet Count Of The Tweet
        /// </summary>

        [Column(Order = 13)]
        public string ChannelTitle { get; set; }

        /// <summary>
        /// Comment Count Of The Tweet
        /// </summary>

        [Column(Order = 14)]
        public int CommentCount { get; set; }

        /// <summary>
        /// True if the tweet has been retweeted
        /// </summary>

        [Column(Order = 15)]
        public int IsRetweet { get; set; }

        /// <summary>
        /// Time when the tweet has been posted in TimeStamp
        /// </summary>

        [Column(Order = 16)]
        public int TweetedTimeStamp { get; set; }

        /// <summary>
        /// Duration of the video tweets
        /// </summary>

        [Column(Order = 17)]
        public string VideoDuration
        { get; set; }

        /// <summary>
        /// View Count of the video tweets
        /// </summary>

        [Column(Order = 18)]
        public int ViewCount
        { get; set; }

        /// <summary>
        /// Image or Video or Text
        /// </summary>

        [Column(Order = 19)]
        public MediaType MediaType { get; set; }

        /// <summary>
        /// If Interaction Type is Comment Interaction
        /// </summary>

        [Column(Order = 20)]
        public string CommentedText
        { get; set; }

        /// <summary>
        /// 1(true) if following Tweet Owner 
        /// </summary>

        [Column(Order = 21)]
        public int FollowStatus
        { get; set; }

        /// <summary>
        /// 1(true) Tweet Owner follows back
        /// </summary>

        [Column(Order = 22)]
        public int FollowBackStatus
        { get; set; }

        /// <summary>
        /// Describes wheather the activity is done in Activity process or after activity process
        /// </summary>
        [Column(Order = 23)]
        public string ProcessType { get; set; }

        [Column(Order = 24)]
        public DateTime InteractionDate { get; set; }

        [Column(Order = 25)]
        public string VideoId { get; set; }

        [Column(Order = 26)]
        public string Query { get; set; }


    }
}
