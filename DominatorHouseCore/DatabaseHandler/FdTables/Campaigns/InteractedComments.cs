using SQLite.CodeFirst;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominatorHouseCore.DatabaseHandler.FdTables.Campaigns
{
    public class InteractedComments
    {

        [Key]
        [Autoincrement]
        [Index]
        [Column(Order = 1)]
        public int Id { get; set; }


        /// <summary>
        /// EmailId of the Account from which Interaction has been done
        /// </summary>
        [Column(Order = 2)]
        public string AccountEmail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 3)]
        public string QueryType
        { get; set; }

        /// <summary>
        /// Contains QueryValue For Interaction
        /// </summary>
        [Column(Order = 4)]
        public string QueryValue { get; set; }

        /// <summary>
        /// 
        /// </summary>        
        [Column(Order = 5)]
        public string ActivityType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 6)]
        public string CommentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 7)]
        public string CommentUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 8)]
        public string CommenterId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 9)]
        public string CommentText { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 10)]
        public string CommetLikeCount { get; set; }

        [Column(Order = 11)]
        public string CommentTimeWithDate { get; set; }

        [Column(Order = 12)]
        public string HasLikedByUser { get; set; }

        [Column(Order = 13)]
        public string CommentPostId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column(Order = 14)]
        public int InteractionTimeStamp { get; set; }


        [Column(Order = 15)]
        public DateTime InteractionDateTime { get; set; }

    }
}
